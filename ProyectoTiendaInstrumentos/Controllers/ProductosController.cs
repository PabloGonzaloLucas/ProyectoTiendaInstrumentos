using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class ProductosController : Controller
    {
        private RepositoryProductos repo;
        public ProductosController(RepositoryProductos repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index(int idSubtipo)
        {
            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
            return View(productos);
        }
    }
}
