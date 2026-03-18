using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using System.Security.Claims;
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
        [AuthorizeUsuarios]
        public async Task<IActionResult> Perfil()
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario user = await this.repo.GetUserByIdAsync(id);
            //if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            //{
            //    return RedirectToAction("Login", "Cuenta");
            //}
            //Usuario user = HttpContext.Session.GetObject<Usuario>("Usuario");

            return View(user);
        }
        [AuthorizeUsuarios]
        public async Task<IActionResult> CambiarPassword()
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario user = await this.repo.GetUserByIdAsync(id);
            //if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            //{
            //    return RedirectToAction("Login", "Cuenta");
            //}
            //Usuario user = HttpContext.Session.GetObject<Usuario>("Usuario");

            return View(user);
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CambiarPassword(string passwordActual, string passwordNueva, string passwordConfirmar)
        {
            //if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            //{
            //    return RedirectToAction("Login", "Cuenta");
            //}

            //Usuario user = HttpContext.Session.GetObject<Usuario>("Usuario");
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario user = await this.repo.GetUserByIdAsync(id);

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
        [ValidateAntiForgeryToken]

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
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario user = await this.repo.LogInUserAsync(email, password);
            if (user == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                   CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role
                );
                if (user.RolId == 1)
                {
                    Claim claimAdmin = new Claim("Admin", "Soy admin");
                    identity.AddClaim(claimAdmin);
                }
                Claim claimName = new Claim(ClaimTypes.Name, user.Nombre);
                identity.AddClaim(claimName);
                Claim claimId = new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString());
                identity.AddClaim(claimId);
                Claim claimRole = new Claim(ClaimTypes.Role, user.RolId.ToString());
                identity.AddClaim(claimRole);
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                HttpContext.Session.SetObject("Usuario", user);
                if (TempData["controller"] != null && TempData["action"] != null)
                {
                    string controller = TempData["controller"].ToString();
                    string action = TempData["action"].ToString();
                    if (action == "Logout")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    if (TempData["id"] != null)
                    {
                        string id = TempData["id"].ToString();
                        return RedirectToAction(action, controller, new { id = id });
                    }
                    else
                    {

                        return RedirectToAction(action, controller);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                ///////////
                //HttpContext.Session.SetObject("Usuario", user);
                // return RedirectToAction("Index", "Home");
            }
        }

        [AuthorizeUsuarios]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (HttpContext.Session.GetObject<Usuario>("Usuario") != null)
            {
                HttpContext.Session.SetObject("Usuario", null);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CambiarFotoPerfil(IFormFile imagen)
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario userSession = await this.repo.GetUserByIdAsync(id);

            bool ok = await this.repo.UpdateUserImageAsync(userSession.IdUsuario, imagen);
            if (!ok)
            {
                TempData["Mensaje"] = "No se pudo actualizar la imagen. Selecciona un archivo válido.";
                return RedirectToAction("Perfil");
            }

            Usuario userActualizado = await this.repo.LogInUserFakePassAsync(userSession.Email, userSession.PasswordFake);
            if (userActualizado != null)
            {
                HttpContext.Session.SetObject("Usuario", userActualizado);
            }

            TempData["Mensaje"] = "Imagen de perfil actualizada correctamente.";
            return RedirectToAction("Perfil");
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ActualizarPerfil(string nombre, string direccion, string telefono)
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario userSession = await this.repo.GetUserByIdAsync(id);

            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Mensaje"] = "El nombre es obligatorio.";
                return RedirectToAction("Perfil");
            }

            bool ok = await this.repo.UpdateUserProfileAsync(userSession.IdUsuario, nombre, direccion, telefono);
            if (!ok)
            {
                TempData["Mensaje"] = "No se pudo actualizar el perfil.";
                return RedirectToAction("Perfil");
            }

            Usuario userActualizado = await this.repo.LogInUserFakePassAsync(userSession.Email, userSession.PasswordFake);
            if (userActualizado != null)
            {
                HttpContext.Session.SetObject("Usuario", userActualizado);
            }

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Perfil");
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> PanelUsuarios()
        {
            List<Usuario> usuarios = await this.repo.GetAllUsersAsync();
            
            foreach(Usuario user in usuarios)
            {
                user.NumPedidos = await this.repo.GetNumComprasUsuario(user.IdUsuario);
            }
            return View(usuarios);
        }
    }
}
