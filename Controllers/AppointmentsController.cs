using BarberiaLosHermanos.Data;
using BarberiaLosHermanos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberiaLosHermanos.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //Solo mostrar citas que NO están facturadas
            var citas = await _context.Appointment
                .Include(a => a.Cliente)
                .Include(a => a.Barbero)
                .Include(a => a.Servicios)
                    .ThenInclude(s => s.Producto)
                .Where(a => !a.Facturada)   // 👈 ESTA ES LA CLAVE
                .OrderBy(a => a.FechaHora)
                .ToListAsync();

            return View(citas);
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await _context.Cliente.ToListAsync();
            ViewBag.Barberos = await _context.Barbero.Where(b => b.Activo).ToListAsync();
            ViewBag.Servicios = await _context.Producto.Where(p => p.Disponible).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ClienteId, int BarberoId, DateTime FechaHora, List<int> serviciosSeleccionados)
        {
            if (serviciosSeleccionados == null || serviciosSeleccionados.Count == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un servicio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = await _context.Cliente.ToListAsync();
                ViewBag.Barberos = await _context.Barbero.Where(b => b.Activo).ToListAsync();
                ViewBag.Servicios = await _context.Producto.Where(p => p.Disponible).ToListAsync();
                return View();
            }
          
            var cita = new Appointment
            {
                ClienteId = ClienteId,
                BarberoId = BarberoId,
                FechaHora = FechaHora,
                Realizada = false
            };

            _context.Appointment.Add(cita);
            await _context.SaveChangesAsync();

            foreach (var s in serviciosSeleccionados)
            {
                _context.AppointmentServicio.Add(new AppointmentServicio
                {
                    AppointmentId = cita.AppointmentId,
                    ProductoId = s
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cita = await _context.Appointment
                .Include(a => a.Servicios)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (cita == null)
                return NotFound();

            ViewBag.Clientes = await _context.Cliente.ToListAsync();
            ViewBag.Barberos = await _context.Barbero.Where(b => b.Activo).ToListAsync();
            ViewBag.Servicios = await _context.Producto.Where(p => p.Disponible).ToListAsync();

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int appointmentId, int ClienteId, int BarberoId, DateTime FechaHora, bool Realizada, List<int> serviciosSeleccionados)
        {
            var cita = await _context.Appointment
                .Include(a => a.Servicios)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (cita == null)
                return NotFound();

            cita.ClienteId = ClienteId;
            cita.BarberoId = BarberoId;
            cita.FechaHora = FechaHora;
            cita.Realizada = Realizada;

            var anteriores = await _context.AppointmentServicio
                .Where(a => a.AppointmentId == appointmentId)
                .ToListAsync();

            _context.AppointmentServicio.RemoveRange(anteriores);

            foreach (var s in serviciosSeleccionados)
            {
                _context.AppointmentServicio.Add(new AppointmentServicio
                {
                    AppointmentId = appointmentId,
                    ProductoId = s
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cita = await _context.Appointment
                .Include(a => a.Cliente)
                .Include(a => a.Barbero)
                .Include(a => a.Servicios)
                    .ThenInclude(s => s.Producto)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (cita == null)
                return NotFound();

            return View(cita);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cita = await _context.Appointment
                .Include(a => a.Cliente)
                .Include(a => a.Barbero)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (cita == null)
                return NotFound();

            return View(cita);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Appointment
                .Include(a => a.Servicios)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (cita != null)
            {
                _context.AppointmentServicio.RemoveRange(cita.Servicios);

                _context.Appointment.Remove(cita);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarRealizada(int id)
        {
            var cita = await _context.Appointment.FindAsync(id);
            if (cita == null)
                return NotFound();

            cita.Realizada = true;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "✔ La cita ha sido marcada como realizada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
