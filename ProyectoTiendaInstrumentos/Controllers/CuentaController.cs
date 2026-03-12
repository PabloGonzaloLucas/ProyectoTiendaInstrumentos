using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using System.Threading.Tasks;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class CuentaController : Controller
    {
        private RepositoryUser repo;

        public CuentaController(RepositoryUser repo)
        {
            this.repo = repo;           
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Perfil()
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            Usuario user = HttpContext.Session.GetObject<Usuario>("Usuario");

            return View(user);
        }
        public async Task<IActionResult> CambiarPassword()
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            Usuario user = HttpContext.Session.GetObject<Usuario>("Usuario");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarPassword(string passwordActual, string passwordNueva, string passwordConfirmar)
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            Usuario user = HttpContext.Session.GetObject<Usuario>("Usuario");

            if (string.IsNullOrWhiteSpace(passwordActual) || string.IsNullOrWhiteSpace(passwordNueva) || string.IsNullOrWhiteSpace(passwordConfirmar))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return View(user);
            }

            if (passwordNueva != passwordConfirmar)
            {
                TempData["Error"] = "La nueva contraseña y la confirmación no coinciden.";
                return View(user);
            }

            if (passwordNueva.Length < 5)
            {
                TempData["Error"] = "La nueva contraseña debe tener al menos 5 caracteres.";
                return View(user);
            }

            bool passwordValida = await this.repo.VerificarPasswordActualAsync(user.IdUsuario, passwordActual);

            if (!passwordValida)
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return View(user);
            }

            bool cambioExitoso = await this.repo.CambiarPasswordAsync(user.IdUsuario, passwordNueva);

            if (cambioExitoso)
            {
                TempData["Success"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("Perfil");
            }
            else
            {
                TempData["Error"] = "Ocurrió un error al cambiar la contraseña. Inténtalo de nuevo.";
                return View(user);
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, IFormFile imagen, string password, string telefono, string direccion)
        {
            await this.repo.RegisterUserFakePassAsync(nombre, email, imagen, password, telefono, direccion);
            ViewBag.MENSAJE = "Usuario en el sistema!!";
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario user = await this.repo.LogInUserFakePassAsync(email, password);
            if (user == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                HttpContext.Session.SetObject("Usuario", user);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if(HttpContext.Session.GetObject<Usuario>("Usuario") != null)
            {
                HttpContext.Session.SetObject("Usuario", null);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
