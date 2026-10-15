using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DespachoApp.Data;
using DespachoApp.Models;

namespace DespachoApp.Controllers
{
    [Authorize]
    public class PedidosController : Controller
    {
        private readonly AppDbContext _db;
        private const string SESSION_KEY = "PedidoActual";

        public PedidosController(AppDbContext db)
        {
            _db = db;
        }

        private PedidoViewModel ObtenerPedidoSesion()
        {
            var json = HttpContext.Session.GetString(SESSION_KEY);
            return json == null ? new PedidoViewModel() : JsonSerializer.Deserialize<PedidoViewModel>(json)!;
        }

        private void GuardarPedidoSesion(PedidoViewModel vm)
        {
            HttpContext.Session.SetString(SESSION_KEY, JsonSerializer.Serialize(vm));
        }

        // Seleccionar cliente

        public async Task<IActionResult> Nuevo()
        {
            HttpContext.Session.Remove(SESSION_KEY); // limpia cualquier pedido anterior
            ViewBag.Clientes = new SelectList(await _db.Clientes.ToListAsync(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SeleccionarCliente(int clienteId)
        {
            var cliente = await _db.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound();

            var vm = new PedidoViewModel { ClienteId = clienteId, NombreCliente = cliente.Nombre };
            GuardarPedidoSesion(vm);
            return RedirectToAction(nameof(Cargar));
        }

        //  Cargar productos

        public IActionResult Cargar()
        {
            var vm = ObtenerPedidoSesion();
            if (vm.ClienteId == 0) return RedirectToAction(nameof(Nuevo));
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarProducto(string codigo, int cantidad)
        {
            var vm = ObtenerPedidoSesion();
            if (vm.ClienteId == 0) return RedirectToAction(nameof(Nuevo));

            if (cantidad <= 0)
            {
                TempData["Error"] = "La cantidad debe ser mayor a 0.";
                return RedirectToAction(nameof(Cargar));
            }

            var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
            if (producto == null)
            {
                TempData["Error"] = $"No se encontró un producto con código '{codigo}'.";
                return RedirectToAction(nameof(Cargar));
            }

            // Si ya está en el detalle, sumar cantidad (no duplicar)
            var linea = vm.Lineas.FirstOrDefault(l => l.ProductoId == producto.Id);
            if (linea != null)
            {
                linea.Cantidad += cantidad;
            }
            else
            {
                vm.Lineas.Add(new LineaPedidoVM
                {
                    ProductoId = producto.Id,
                    Codigo = producto.Codigo,
                    Descripcion = producto.Descripcion,
                    Cantidad = cantidad,
                    CostoUnitario = producto.CostoUnitario ?? 0
                });
            }

            GuardarPedidoSesion(vm);
            return RedirectToAction(nameof(Cargar));
        }

        [HttpPost]
        public IActionResult EliminarLinea(int productoId)
        {
            var vm = ObtenerPedidoSesion();
            vm.Lineas.RemoveAll(l => l.ProductoId == productoId);
            GuardarPedidoSesion(vm);
            return RedirectToAction(nameof(Cargar));
        }

        // Confirmar pedido

        [HttpPost]
        public async Task<IActionResult> Confirmar()
        {
            var vm = ObtenerPedidoSesion();
            if (vm.ClienteId == 0 || !vm.Lineas.Any())
            {
                TempData["Error"] = "El pedido está vacío.";
                return RedirectToAction(nameof(Cargar));
            }

            // Obtener el id del usuario logueado desde los claims
            var usuarioIdClaim = User.FindFirst("UsuarioId")?.Value;
            if (!int.TryParse(usuarioIdClaim, out int usuarioId))
                return Forbid();

            var pedido = new Pedido
            {
                Fecha = DateTime.Now,
                ClienteId = vm.ClienteId,
                UsuarioId = usuarioId,
                Confirmado = true,
                Total = vm.MontoTotal,
                Detalles = vm.Lineas.Select(l => new DetallePedido
                {
                    ProductoId = l.ProductoId,
                    Cantidad = l.Cantidad,
                    CostoUnitario = l.CostoUnitario
                }).ToList()
            };

            foreach (var linea in vm.Lineas)
            {
                var producto = await _db.Productos.FindAsync(linea.ProductoId);

                if (producto == null)
                    continue;

                if (producto.Stock < linea.Cantidad)
                {
                    TempData["Error"] =
                        $"No hay stock suficiente para {producto.Descripcion}";

                    return RedirectToAction(nameof(Cargar));
                }

                producto.Stock -= linea.Cantidad;
            }

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();
            HttpContext.Session.Remove(SESSION_KEY);

            TempData["Exito"] = $"Pedido #{pedido.Id} confirmado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Listado de pedidos

        public async Task<IActionResult> Index(string? buscarUsuario)
        {
            var pedidos = _db.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(buscarUsuario))
                pedidos = pedidos.Where(p => p.Usuario!.NombreUsuario.Contains(buscarUsuario));

            ViewBag.BuscarUsuario = buscarUsuario;
            return View(await pedidos.OrderByDescending(p => p.Fecha).ToListAsync());
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();
            return View(pedido);
        }
    }
}
