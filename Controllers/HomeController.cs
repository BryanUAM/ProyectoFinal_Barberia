using BarberiaLosHermanos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberiaLosHermanos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalClientes = await _context.Cliente.CountAsync();
            ViewBag.TotalServicios = await _context.Producto.CountAsync(p => p.Disponible == true);

            // Solo citas NO realizadas y NO facturadas
            ViewBag.TotalCitas = await _context.Appointment
                .Where(c => !c.Realizada && !c.Facturada)
                .CountAsync();

            // Citas pendientes (no realizadas y no facturadas)
            var citasPendientes = await _context.Appointment
                .Include(c => c.Cliente)
                .Include(c => c.Barbero)
                .Include(c => c.Servicios).ThenInclude(s => s.Producto)
                .Where(c => !c.Realizada && !c.Facturada)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();

            // Citas realizadas pero aún SIN factura
            var citasRealizadas = await _context.Appointment
                .Include(c => c.Cliente)
                .Include(c => c.Barbero)
                .Include(c => c.Servicios).ThenInclude(s => s.Producto)
                .Where(c => c.Realizada && !c.Facturada)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();

            ViewBag.CitasPendientes = citasPendientes;
            ViewBag.CitasRealizadas = citasRealizadas;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCitasRealizadas()
        {
            var realizadas = await _context.Appointment
                .Include(a => a.Servicios)
                .Where(c => c.Realizada)
                .ToListAsync();

            if (realizadas.Any())
            {
                foreach (var cita in realizadas)
                {
                    _context.AppointmentServicio.RemoveRange(cita.Servicios);
                }

                _context.Appointment.RemoveRange(realizadas);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = $"{realizadas.Count} Cita(s) realizadas fueron eliminadas correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No hay citas realizadas para eliminar.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
