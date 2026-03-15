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
            List<Tipo> tipos = await this.repo.GetTiposByFamiliaAsync(idFamilia);
            ViewBag.idFamilia = idFamilia;
            return View(tipos);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Index(string Nombre, int IdFamilia, string accion)
        {
            if(accion == "cancelar")
            {
                
            }
            else if(accion == "add")
            {
                await this.repo.InsertTipoAsync(Nombre, IdFamilia);
            }
            ViewBag.idFamilia = IdFamilia;
            List<Tipo> tipos = await this.repo.GetTiposByFamiliaAsync(IdFamilia);
            return View(tipos);

        }
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> Eliminar(int idTipo, int idFamilia)
        {
            await this.repo.DeleteTipoAsync(idTipo);
            //ViewBag.Tipo = await this.repo.GetTipoByIdAsync(idTipo); 
            //List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(idTipo);
            return RedirectToAction("Index", "Tipos", new { idFamilia = idFamilia });
        }
    }
}
