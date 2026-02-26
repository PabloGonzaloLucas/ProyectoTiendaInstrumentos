using Microsoft.AspNetCore.Mvc;
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


            return View(tipos);
        }
    }
}
