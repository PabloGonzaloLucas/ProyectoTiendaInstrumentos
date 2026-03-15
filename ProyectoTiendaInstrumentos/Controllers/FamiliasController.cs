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
            await this.repo.InsertFamiliaAsync(nombre, imagenBanner);

            // Vuelve a la página desde la que se envió el formulario
            string? referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Eliminar(int idFamilia)
        {
            await this.repo.DeleteFamiliaAsync(idFamilia);
            string? referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
