using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class SubtiposController : Controller
    {
        private RepositorySubtipos repo;
        public SubtiposController(RepositorySubtipos repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index(int idTipo)
        {
            try
            {
                if (idTipo <= 0)
                {
                    TempData["Error"] = "Identificador de tipo inválido.";
                    ViewBag.Tipo = null;
                    return View(new List<Subtipo>());
                }

                ViewBag.Tipo = await this.repo.GetTipoByIdAsync(idTipo);
                List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(idTipo);
                return View(subtipos);
            }
            catch
            {
                TempData["Error"] = "No se pudieron cargar los subtipos.";
                ViewBag.Tipo = null;
                return View(new List<Subtipo>());
            }
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Eliminar(int idSubtipo, int idTipo)
        {
            TempData["Error"] = "Operación no permitida por GET. Confirma la eliminación desde el botón.";
            return RedirectToAction("Index", "Subtipos", new { idTipo = idTipo });
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPost(int idSubtipo, int idTipo)
        {
            try
            {
                if (idSubtipo <= 0 || idTipo <= 0)
                {
                    TempData["Error"] = "Parámetros inválidos para eliminar.";
                    return RedirectToAction("Index", "Subtipos", new { idTipo = idTipo });
                }

                await this.repo.DeleteSubtipoAsync(idSubtipo);
                TempData["Success"] = "Subtipo eliminado correctamente.";
                return RedirectToAction("Index", "Subtipos", new { idTipo = idTipo });
            }
            catch
            {
                TempData["Error"] = "No se pudo eliminar el subtipo.";
                return RedirectToAction("Index", "Subtipos", new { idTipo = idTipo });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string Nombre, int IdTipo, string accion)
        {
            try
            {
                if (IdTipo <= 0)
                {
                    TempData["Error"] = "Identificador de tipo inválido.";
                    ViewBag.Tipo = null;
                    return View(new List<Subtipo>());
                }

                if (accion == "cancelar")
                {
                    // no-op
                }
                else if (accion == "add")
                {
                    if (string.IsNullOrWhiteSpace(Nombre))
                    {
                        TempData["Error"] = "El nombre del subtipo es obligatorio.";
                    }
                    else
                    {
                        await this.repo.InsertSubtipoAsync(Nombre, IdTipo);
                        TempData["Success"] = "Subtipo añadido correctamente.";
                    }
                }

                ViewBag.Tipo = await this.repo.GetTipoByIdAsync(IdTipo);
                List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(IdTipo);
                return View(subtipos);
            }
            catch
            {
                TempData["Error"] = "No se pudo procesar la operación.";
                ViewBag.Tipo = await this.repo.GetTipoByIdAsync(IdTipo);
                List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(IdTipo);
                return View(subtipos);
            }
        }

    }
}
