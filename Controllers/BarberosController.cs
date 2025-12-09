using BarberiaLosHermanos.Data;
using BarberiaLosHermanos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberiaLosHermanos.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BarberosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarberosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var barberos = await _context.Barbero.ToListAsync();
            return View(barberos);
        }

        // GET: Crear
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Barbero barbero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(barbero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(barbero);
        }

        // GET: Editar
        public async Task<IActionResult> Edit(int id)
        {
            var barbero = await _context.Barbero.FindAsync(id);
            if (barbero == null)
                return NotFound();

            return View(barbero);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Barbero barbero)
        {
            if (id != barbero.BarberoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(barbero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(barbero);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var barbero = await _context.Barbero.FindAsync(id);
            if (barbero == null)
                return NotFound();

            return View(barbero);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barbero = await _context.Barbero.FindAsync(id);
            if (barbero != null)
            {
                _context.Barbero.Remove(barbero);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
