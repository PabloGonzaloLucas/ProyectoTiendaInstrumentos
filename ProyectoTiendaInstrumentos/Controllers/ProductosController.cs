using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class ProductosController : Controller
    {
        private RepositoryProductos repo;
        private IRepositoryCarrito repoCarrito;
        public ProductosController(RepositoryProductos repo, IRepositoryCarrito repositoryCarrito)
        {
            this.repo = repo;
            this.repoCarrito = repositoryCarrito;
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

        [HttpPost]
        public async Task<IActionResult> AddToCart(int idProducto, int cantidad = 1)
        {
            if(HttpContext.Session.GetObject<Usuario>("Usuario") != null)
            {
                int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
                await this.repoCarrito.AddProductoToCartAsync(idProducto, idUsuario,cantidad);
                return RedirectToAction("Details", new { idProducto = idProducto });
            }
            else
            {
                return RedirectToAction("Login", "Cuenta");
            }
        }
    }
}
