using BarberiaLosHermanos.Data;
using BarberiaLosHermanos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberiaLosHermanos.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class AgendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Barberos = await _context.Barbero.Where(b => b.Activo).ToListAsync();
            ViewBag.Servicios = await _context.Producto.Where(p => p.Disponible).ToListAsync();

            return View();
        }


        public async Task<IActionResult> MisCitas()
        {
            // Obtener correo del usuario logueado
            var correo = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            // Buscar cliente asociado (mismo código que Confirmar)
            var cliente = _context.Cliente.FirstOrDefault(c => c.Nombre == usuario.Nombre);

            if (cliente == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var citas = await _context.Appointment
                .Include(a => a.Barbero)
                .Include(a => a.Servicios).ThenInclude(s => s.Producto)
                .Where(a => a.ClienteId == cliente.ClienteId)
                .OrderByDescending(a => a.FechaHora)
                .ToListAsync();

            return View(citas);
        }

        public async Task<IActionResult> Horarios(int barberoId, DateTime fecha, List<int> serviciosSeleccionados)
        {
            var hoy = DateTime.Today;

            if (fecha.Date < hoy)
                return BadRequest("La fecha no puede ser pasada.");

            if (fecha.Date > hoy.AddDays(7))
                return BadRequest("Solo se permiten citas dentro de los próximos 7 días.");

            TimeSpan apertura = new TimeSpan(9, 0, 0);
            TimeSpan cierre = new TimeSpan(18, 0, 0);
            int intervalo = 30;

            var citas = await _context.Appointment
                .Where(a => a.BarberoId == barberoId && a.FechaHora.Date == fecha.Date)
                .Select(a => a.FechaHora.TimeOfDay)
                .ToListAsync();

            List<TimeSpan> horarios = new();

            for (var h = apertura; h <= cierre; h = h.Add(TimeSpan.FromMinutes(intervalo)))
            {
                if (!citas.Contains(h))
                    horarios.Add(h);
            }

            // Oculta horas ya pasadas de HOY
            if (fecha.Date == DateTime.Today)
            {
                var horaActual = DateTime.Now.TimeOfDay;

                horarios = horarios
                    .Where(h => h > horaActual)
                    .ToList();
            }

            ViewBag.BarberoId = barberoId;
            ViewBag.Fecha = fecha;
            ViewBag.ServiciosSeleccionados = serviciosSeleccionados;

            return View(horarios);
        }


        [HttpPost]
        public async Task<IActionResult> Confirmar(int barberoId, DateTime fecha, TimeSpan hora, List<int> serviciosSeleccionados)
        {
            if (barberoId == 0)
            {
                TempData["Error"] = "Debe seleccionar un barbero.";
                return RedirectToAction("Index");
            }

            if (serviciosSeleccionados == null || !serviciosSeleccionados.Any())
            {
                TempData["Error"] = "Debe seleccionar al menos un servicio.";
                return RedirectToAction("Index");
            }

            var correo = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);
            var cliente = _context.Cliente.FirstOrDefault(c => c.Nombre == usuario.Nombre);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nombre = usuario.Nombre,
                    Email = usuario.Correo,
                    Telefono = "0000-0000"
                };

                _context.Cliente.Add(cliente);
                await _context.SaveChangesAsync();
            }

            ViewBag.ClienteId = cliente.ClienteId;
            ViewBag.ClienteNombre = cliente.Nombre;

            ViewBag.Barbero = await _context.Barbero.FindAsync(barberoId);
            ViewBag.Servicios = await _context.Producto.Where(p => serviciosSeleccionados.Contains(p.ProductoId)).ToListAsync();
            ViewBag.FechaHora = fecha.Date + hora;
            ViewBag.ServiciosSeleccionados = serviciosSeleccionados;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgendarCita(int clienteId, int barberoId, DateTime fechaHora, List<int> serviciosSeleccionados)
        {
            if (serviciosSeleccionados == null || serviciosSeleccionados.Count == 0)
            {
                TempData["Error"] = "Debe seleccionar al menos un servicio.";
                return RedirectToAction("Index");
            }

            bool ocupado = await _context.Appointment
                .AnyAsync(a => a.BarberoId == barberoId && a.FechaHora == fechaHora);

            if (ocupado)
            {
                TempData["Error"] = "El barbero ya tiene una cita en ese horario.";
                return RedirectToAction("Index");
            }

            Appointment cita = new Appointment
            {
                ClienteId = clienteId,
                BarberoId = barberoId,
                FechaHora = fechaHora,
                Realizada = false
            };

            _context.Appointment.Add(cita);
            await _context.SaveChangesAsync();

            foreach (var servicioId in serviciosSeleccionados)
            {
                _context.AppointmentServicio.Add(new AppointmentServicio
                {
                    AppointmentId = cita.AppointmentId,
                    ProductoId = servicioId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("CitaAgregada");
        }

        public IActionResult CitaAgregada()
        {
            return View();
        }
    }
}

