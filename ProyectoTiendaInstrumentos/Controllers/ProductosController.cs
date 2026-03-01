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
            ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
            return View(productos);
        }

        public async Task<IActionResult> Details(int idProducto)
        {
            ViewBag.ImagenesProductos = await this.repo.GetImagenesProductoByIdAsync(idProducto);
            ViewBag.Especificaciones = await this.repo.GetEspecificacionesAsync(idProducto);
            VwDetallesProducto producto = await this.repo.GetDetallesProductoAsync(idProducto);
            return View(producto);
        }
    }
}
