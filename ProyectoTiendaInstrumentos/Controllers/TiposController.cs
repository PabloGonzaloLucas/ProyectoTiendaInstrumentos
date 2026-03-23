using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class TiposController : Controller
    {
        private RepositoryTipos repo;
        public TiposController(RepositoryTipos repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index(int idFamilia)
        {
            try
            {
                if (idFamilia <= 0)
                {
                    TempData["Error"] = "Identificador de familia inválido.";
                    return View(new List<Tipo>());
                }

                List<Tipo> tipos = await this.repo.GetTiposByFamiliaAsync(idFamilia);
                ViewBag.idFamilia = idFamilia;
                return View(tipos);
            }
            catch
            {
                TempData["Error"] = "No se pudieron cargar los tipos.";
                ViewBag.idFamilia = idFamilia;
                return View(new List<Tipo>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string Nombre, int IdFamilia, string accion)
        {
            try
            {
                if (IdFamilia <= 0)
                {
                    TempData["Error"] = "Identificador de familia inválido.";
                    ViewBag.idFamilia = 0;
                    return View(new List<Tipo>());
                }

                if (accion == "cancelar")
                {
                    // no-op
                }
                else if (accion == "add")
                {
                    if (string.IsNullOrWhiteSpace(Nombre))
                    {
                        TempData["Error"] = "El nombre del tipo es obligatorio.";
                    }
                    else
                    {
                        await this.repo.InsertTipoAsync(Nombre, IdFamilia);
                        TempData["Success"] = "Tipo añadido correctamente.";
                    }
                }

                ViewBag.idFamilia = IdFamilia;
                List<Tipo> tipos = await this.repo.GetTiposByFamiliaAsync(IdFamilia);
                return View(tipos);
            }
            catch
            {
                TempData["Error"] = "No se pudo procesar la operación.";
                ViewBag.idFamilia = IdFamilia;
                List<Tipo> tipos = await this.repo.GetTiposByFamiliaAsync(IdFamilia);
                return View(tipos);
            }
        }

        // Compatibilidad: si algún enlace antiguo sigue llamando por GET, redirigimos al listado.
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Eliminar(int idTipo, int idFamilia)
        {
            TempData["Error"] = "Operación no permitida por GET. Confirma la eliminación desde el botón.";
            return RedirectToAction("Index", "Tipos", new { idFamilia = idFamilia });
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPost(int idTipo, int idFamilia)
        {
            try
            {
                if (idTipo <= 0 || idFamilia <= 0)
                {
                    TempData["Error"] = "Parámetros inválidos para eliminar.";
                    return RedirectToAction("Index", "Tipos", new { idFamilia = idFamilia });
                }

                await this.repo.DeleteTipoAsync(idTipo);
                TempData["Success"] = "Tipo eliminado correctamente.";
                return RedirectToAction("Index", "Tipos", new { idFamilia = idFamilia });
            }
            catch
            {
                TempData["Error"] = "No se pudo eliminar el tipo. Es posible que haya subtipos asociados.";
                return RedirectToAction("Index", "Tipos", new { idFamilia = idFamilia });
            }
        }
    }
}
