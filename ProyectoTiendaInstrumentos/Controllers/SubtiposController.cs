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
            ViewBag.Tipo = await this.repo.GetTipoByIdAsync(idTipo); 
            List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(idTipo);

            return View(subtipos);
        }
        [AuthorizeUsuarios(Policy = "AdminOnly")]

        public async Task<IActionResult> Eliminar(int idSubtipo, int idTipo)
        {
            await this.repo.DeleteSubtipoAsync(idSubtipo);
            //ViewBag.Tipo = await this.repo.GetTipoByIdAsync(idTipo); 
            //List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(idTipo);
            return RedirectToAction("Index", "Subtipos", new { idTipo = idTipo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Index(string Nombre, int IdTipo, string accion)
        {
            if (accion == "cancelar")
            {

            }
            else if (accion == "add")
            {
                await this.repo.InsertSubtipoAsync(Nombre, IdTipo);
            }
            ViewBag.Tipo = await this.repo.GetTipoByIdAsync(IdTipo);
            List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(IdTipo);
            return View(subtipos);
        }

    }
}
