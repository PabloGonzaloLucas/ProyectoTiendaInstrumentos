using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Repositories;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class FamiliasController : Controller
    {
        private readonly RepositoryTipos repo;

        public FamiliasController(RepositoryTipos repo)
        {
            this.repo = repo;
        }

        [HttpPost]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombre, IFormFile imagenBanner)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    TempData["Error"] = "El nombre de la familia es obligatorio.";
                    return RedirectBackOrHome();
                }

                await this.repo.InsertFamiliaAsync(nombre, imagenBanner);
                TempData["Success"] = "Familia creada correctamente.";
                return RedirectBackOrHome();
            }
            catch
            {
                TempData["Error"] = "No se pudo crear la familia. Inténtalo de nuevo.";
                return RedirectBackOrHome();
            }
        }

        // GET: compatibilidad / evitar borrados por enlace
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Eliminar(int idFamilia)
        {
            TempData["Error"] = "Operación no permitida por GET. Confirma la eliminación desde el botón.";
            return RedirectBackOrHome();
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPost(int idFamilia)
        {
            try
            {
                if (idFamilia <= 0)
                {
                    TempData["Error"] = "Identificador de familia inválido.";
                    return RedirectBackOrHome();
                }

                await this.repo.DeleteFamiliaAsync(idFamilia);
                TempData["Success"] = "Familia eliminada correctamente.";
                return RedirectBackOrHome();
            }
            catch
            {
                TempData["Error"] = "No se pudo eliminar la familia. Es posible que tenga tipos asociados.";
                return RedirectBackOrHome();
            }
        }

        private IActionResult RedirectBackOrHome()
        {
            string? referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
