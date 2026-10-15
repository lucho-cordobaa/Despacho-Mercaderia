using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DespachoApp.Data;
using DespachoApp.Models;
using BCrypt.Net;

namespace DespachoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _db;

        public UsuariosController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index() => View(await _db.Usuarios.ToListAsync());

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();
            if (!ModelState.IsValid) return View(usuario);

            // Si el campo password viene vacío, conservar el hash actual
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                var usuarioActual = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (usuarioActual == null) return NotFound();
                usuario.Password = usuarioActual.Password;
            }
            else
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }

            _db.Usuarios.Update(usuario);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _db.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            bool tienePedidos = await _db.Pedidos.AnyAsync(p => p.UsuarioId == id);

            if (tienePedidos)
            {
                TempData["Error"] = "No se puede eliminar el usuario porque tiene pedidos registrados.";
                return RedirectToAction(nameof(Index));
            }

            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();

            TempData["Exito"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
