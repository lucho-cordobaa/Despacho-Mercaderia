using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DespachoApp.Data;
using DespachoApp.Models;

namespace DespachoApp.Controllers
{
    [Authorize]
    public class DireccionesController : Controller
    {
        private readonly AppDbContext _db;

        public DireccionesController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? clienteId)
        {
            var dirs = _db.Direcciones.Include(d => d.Cliente).AsQueryable();
            if (clienteId.HasValue)
                dirs = dirs.Where(d => d.ClienteId == clienteId);
            ViewBag.ClienteId = clienteId;
            return View(await dirs.ToListAsync());
        }

        public async Task<IActionResult> Create(int? clienteId)
        {
            ViewBag.ClienteId = new SelectList(await _db.Clientes.ToListAsync(), "Id", "Nombre", clienteId);
            return View(new Direccion { ClienteId = clienteId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Direccion dir)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClienteId = new SelectList(await _db.Clientes.ToListAsync(), "Id", "Nombre", dir.ClienteId);
                return View(dir);
            }
            _db.Direcciones.Add(dir);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Dirección agregada correctamente.";
            return RedirectToAction("Details", "Clientes", new { id = dir.ClienteId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dir = await _db.Direcciones.FindAsync(id);
            if (dir == null) return NotFound();
            ViewBag.ClienteId = new SelectList(await _db.Clientes.ToListAsync(), "Id", "Nombre", dir.ClienteId);
            return View(dir);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Direccion dir)
        {
            if (id != dir.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.ClienteId = new SelectList(await _db.Clientes.ToListAsync(), "Id", "Nombre", dir.ClienteId);
                return View(dir);
            }
            _db.Direcciones.Update(dir);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Dirección actualizada correctamente.";
            return RedirectToAction("Details", "Clientes", new { id = dir.ClienteId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dir = await _db.Direcciones.FindAsync(id);
            if (dir == null) return NotFound();
            int clienteId = dir.ClienteId;
            _db.Direcciones.Remove(dir);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Dirección eliminada correctamente.";
            return RedirectToAction("Details", "Clientes", new { id = clienteId });
        }
    }
}
