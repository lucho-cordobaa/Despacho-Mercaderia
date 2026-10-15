using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DespachoApp.Data;
using DespachoApp.Models;

namespace DespachoApp.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly AppDbContext _db;

        public ProductosController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            var productos = _db.Productos.AsQueryable();
            if (!string.IsNullOrEmpty(buscar))
                productos = productos.Where(p => p.Codigo.Contains(buscar) || p.Descripcion.Contains(buscar));
            ViewBag.Buscar = buscar;
            return View(await productos.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);
            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.Id) return NotFound();
            if (!ModelState.IsValid) return View(producto);
            _db.Productos.Update(producto);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return NotFound();
            _db.Productos.Remove(p);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Endpoint para buscar producto por código
        [HttpGet]
        public async Task<IActionResult> BuscarPorCodigo(string codigo)
        {
            var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
            if (producto == null)
                return Json(new { ok = false, mensaje = "Producto no encontrado." });
            return Json(new { ok = true, id = producto.Id, codigo = producto.Codigo, descripcion = producto.Descripcion, costoUnitario = producto.CostoUnitario });
        }
    }
}
