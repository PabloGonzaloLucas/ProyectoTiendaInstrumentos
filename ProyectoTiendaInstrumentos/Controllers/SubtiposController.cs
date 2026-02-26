using Microsoft.AspNetCore.Mvc;
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
            List<Subtipo> subtipos = await this.repo.GetSubtiposByTipoAsync(idTipo);


            return View(subtipos);
        }
    }
}
