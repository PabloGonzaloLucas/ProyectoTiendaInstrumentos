using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;

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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string imagen, string password, string telefono, string direccion)
        {
            await this.repo.RegisterUserFakePassAsync(nombre, email, imagen, password, telefono, direccion);
            ViewBag.MENSAJE = "Usuario en el sistema!!";
            return View();
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
                return View(user);
            }
        }
    }
}
