using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DespachoApp.Data;
using DespachoApp.Models;

namespace DespachoApp.Controllers
{
    // tiene que estar autenticado para acceder a cualquier acción de este controlador
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly AppDbContext _db;

        public ClientesController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            var clientes = _db.Clientes.AsQueryable();
            if (!string.IsNullOrEmpty(buscar))
                clientes = clientes.Where(c => c.Nombre.Contains(buscar) || c.DniCuit.Contains(buscar));
            ViewBag.Buscar = buscar;
            return View(await clientes.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _db.Clientes
                .Include(c => c.Direcciones)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            // verifica las validaciones
            if (!ModelState.IsValid) return View(cliente);
            _db.Clientes.Add(cliente);
            // aca guarda los datos (INSERT)
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Cliente creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();
            if (!ModelState.IsValid) return View(cliente);
            // marca todos los campos como modificados
            _db.Clientes.Update(cliente);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // muestra la vista de confirmación de eliminación
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            bool tienePedidos = await _db.Pedidos.AnyAsync(p => p.ClienteId == id);

            if (tienePedidos)
            {
                TempData["Error"] = "No se puede eliminar el cliente porque tiene pedidos registrados.";
                return RedirectToAction(nameof(Index));
            }

            _db.Clientes.Remove(cliente);
            await _db.SaveChangesAsync();

            TempData["Exito"] = "Cliente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
