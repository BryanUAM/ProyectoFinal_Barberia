using BarberiaLosHermanos.Data;
using BarberiaLosHermanos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberiaLosHermanos.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacturasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var facturas = await _context.Factura
                .Include(f => f.Cliente)
                .ToListAsync();

            return View(facturas);
        }

        // 🔥 NUEVA FACTURA A PARTIR DE UNA CITA
        public async Task<IActionResult> Create(int appointmentId)
        {
            var cita = await _context.Appointment
                .Include(a => a.Cliente)
                .Include(a => a.Barbero)
                .Include(a => a.Servicios)
                    .ThenInclude(s => s.Producto)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (cita == null)
                return RedirectToAction("Index", "Appointments");

            ViewBag.Cliente = cita.Cliente.Nombre;
            ViewBag.Barbero = cita.Barbero.Nombre;

            ViewBag.Servicios = cita.Servicios.Select(s => new
            {
                nombre = s.Producto.Nombre,
                precio = s.Producto.Precio
            }).ToList();

            ViewBag.Total = cita.Servicios.Sum(s => s.Producto.Precio);
            ViewBag.AppointmentId = cita.AppointmentId;

            return View();
        }

        // 🔥 GUARDAR FACTURA GENERADA AUTOMÁTICAMENTE
        [HttpPost]
        public async Task<IActionResult> CreateFactura(int appointmentId)
        {
            var cita = await _context.Appointment
                 .Include(a => a.Cliente)                 // ⭐ NECESARIO
                 .Include(a => a.Servicios)
                     .ThenInclude(s => s.Producto)        // ⭐ NECESARIO
                 .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (cita == null)
            {
                TempData["Error"] = "No se pudo encontrar la cita asociada a esta factura.";
                return RedirectToAction("Index");
            }


            // 2. Crear la factura
            var factura = new Factura
            {
                ClienteId = cita.ClienteId,
                Fecha = DateTime.Now,
                MontoTotal = cita.Servicios.Sum(s => s.Producto.Precio)
            };

            _context.Factura.Add(factura);
            await _context.SaveChangesAsync();

            // 3. Marcar la cita como facturada
            cita.Facturada = true;
            await _context.SaveChangesAsync();

            // 4. Guardar los servicios en ProductosFactura
            foreach (var servicio in cita.Servicios)
            {
                _context.ProductosFactura.Add(new ProductosFactura
                {
                    FacturaId = factura.FacturaId,
                    ProductoId = servicio.ProductoId
                });
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Factura generada correctamente.";
            return RedirectToAction("Index");
        }


        // DETALLES
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // Traemos factura con cliente y sus servicios
            var factura = await _context.Factura
                .Include(f => f.Cliente)
                .Include(f => f.ProductosFacturas)
                    .ThenInclude(pf => pf.Producto)
                .FirstOrDefaultAsync(f => f.FacturaId == id);

            if (factura == null)
                return NotFound();

            // 🔥 Buscar la cita REAL asociada a esta factura
            // 1. Buscar un ProductoFactura de esta factura
            var primerServicio = factura.ProductosFacturas.FirstOrDefault();

            if (primerServicio != null)
            {
                // 2. Buscar la relación AppointmentServicio que tenga ese producto
                var relacion = await _context.AppointmentServicio
                    .Include(asv => asv.Appointment)
                        .ThenInclude(a => a.Barbero)
                    .FirstOrDefaultAsync(asv => asv.ProductoId == primerServicio.ProductoId
                                                && asv.Appointment.ClienteId == factura.ClienteId);

                if (relacion != null)
                {
                    ViewBag.Barbero = relacion.Appointment.Barbero.Nombre;
                }
                else
                {
                    ViewBag.Barbero = "No disponible";
                }
            }
            else
            {
                ViewBag.Barbero = "No disponible";
            }

            return View(factura);
        }




        // EDITAR
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var factura = await _context.Factura.FindAsync(id);
            if (factura == null)
                return NotFound();

            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "Nombre", factura.ClienteId);
            return View(factura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacturaId,Fecha,ClienteId,MontoTotal")] Factura factura)
        {
            if (id != factura.FacturaId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(factura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "Nombre", factura.ClienteId);
            return View(factura);
        }

        // ELIMINAR
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var factura = await _context.Factura
                .Include(f => f.Cliente)
                .FirstOrDefaultAsync(f => f.FacturaId == id);

            if (factura == null)
                return NotFound();

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Factura
                .Include(f => f.ProductosFacturas)
                .FirstOrDefaultAsync(f => f.FacturaId == id);

            if (factura != null)
            {
                // Eliminar primero el detalle (ProductosFactura)
                if (factura.ProductosFacturas != null && factura.ProductosFacturas.Any())
                {
                    _context.ProductosFactura.RemoveRange(factura.ProductosFacturas);
                }

                // Luego eliminar la factura
                _context.Factura.Remove(factura);

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Factura eliminada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
