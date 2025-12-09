using BarberiaLosHermanos.Data;
using BarberiaLosHermanos.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarberiaLosHermanos.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------
        // GET: /Account/Login
        // ---------------------------------------
        public IActionResult Login()
        {
            return View();
        }


        // ---------------------------------------
        // POST: /Account/Login
        // ---------------------------------------
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            // Buscar el usuario por correo
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario == null)
            {
                TempData["Error"] = "El usuario no existe.";
                return RedirectToAction("Login");
            }

            // Verificar contraseña 
            if (usuario.Contrasena != contrasena)
            {
                TempData["Error"] = "Contraseña incorrecta.";
                return RedirectToAction("Login");
            }

            // Crear claims (Nombre, Correo, Rol)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol) // Admin o Cliente
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // Iniciar sesión
            await HttpContext.SignInAsync("CookieAuth", principal);

            if (usuario.Rol == "Admin")
            {
                return RedirectToAction("Index", "Admin");  // Panel administrador
            }
            else
            {
                return RedirectToAction("Index", "Agendar");   // Página principal cliente
            }
        }

        // ---------------------------------------
        // GET: /Account/Registrar
        // ---------------------------------------
        public IActionResult Registrar()
        {
            return View();
        }

        // ---------------------------------------
        // POST: /Account/Registrar
        // ---------------------------------------
        [HttpPost]
        public IActionResult Registrar(string Nombre, string Correo, string Telefono, string Contrasena)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Contrasena))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return View();
            }

            var usuario = new Usuario
            {
                Nombre = Nombre,
                Correo = Correo,
                Telefono = Telefono,
                Contrasena = Contrasena,
                Rol = "Cliente"
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var cliente = new Cliente
            {
                Nombre = Nombre,
                Email = Correo,
                Telefono = Telefono
            };

            _context.Cliente.Add(cliente);
            _context.SaveChanges();

            TempData["Mensaje"] = "Se ha registrado un nuevo Cliente correctamente.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        public IActionResult Denegado()
        {
            return View();
        }
    }
}
