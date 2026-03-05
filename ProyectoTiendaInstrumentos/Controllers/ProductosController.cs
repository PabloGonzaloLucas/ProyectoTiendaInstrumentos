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
        private RepositoryValoraciones repoValoraciones;
        private IRepositoryCarrito repoCarrito;
        public ProductosController(RepositoryProductos repo, IRepositoryCarrito repositoryCarrito, RepositoryValoraciones repoValoraciones)
        {
            this.repo = repo;
            this.repoCarrito = repositoryCarrito;
            this.repoValoraciones = repoValoraciones;
        }
        public async Task<IActionResult> Index(int idSubtipo, string q = null)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                List<VwCatalogoProducto> resultados = await this.repo.BuscarProductosPorNombreAsync(q);
                ViewBag.TerminoBusqueda = q;
                ViewBag.NumeroResultados = resultados.Count;
                return View(resultados);
            }

            ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
            return View(productos);
        }

        public async Task<IActionResult> Buscar(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                TempData["Mensaje"] = "Por favor, introduce un término de búsqueda.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", new { q });
        }

        public async Task<IActionResult> Details(int idProducto)
        {
            ViewBag.ImagenesProductos = await this.repo.GetImagenesProductoByIdAsync(idProducto);
            ViewBag.Especificaciones = await this.repo.GetEspecificacionesAsync(idProducto);
            ViewBag.Valoraciones = await this.repoValoraciones.GetValoracionesByProductoAsync(idProducto);
            
            if (HttpContext.Session.GetObject<Usuario>("Usuario") != null)
            {
                int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
                ViewBag.UsuarioHaValorado = await this.repoValoraciones.UsuarioHaValoradoProductoAsync(idUsuario, idProducto);
                ViewBag.UsuarioHaComprado = await this.repoValoraciones.UsuarioHaCompradoProductoAsync(idUsuario, idProducto);
            }
            else
            {
                ViewBag.UsuarioHaValorado = false;
                ViewBag.UsuarioHaComprado = false;
            }

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

        [HttpPost]
        public async Task<IActionResult> AddValoracion(int idProducto, int puntuacion, string comentario)
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                TempData["ErrorValoracion"] = "Debes iniciar sesión para dejar una reseña.";
                return RedirectToAction("Details", new { idProducto = idProducto });
            }

            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;

            if (puntuacion < 1 || puntuacion > 5)
            {
                TempData["ErrorValoracion"] = "La puntuación debe estar entre 1 y 5.";
                return RedirectToAction("Details", new { idProducto = idProducto });
            }

            if (string.IsNullOrWhiteSpace(comentario))
            {
                TempData["ErrorValoracion"] = "El comentario no puede estar vacío.";
                return RedirectToAction("Details", new { idProducto = idProducto });
            }

            int resultado = await this.repoValoraciones.AddValoracionAsync(idUsuario, idProducto, puntuacion, comentario);

            if (resultado == 0)
            {
                TempData["ErrorValoracion"] = "Ya has valorado este producto anteriormente.";
            }
            else if (resultado == -1)
            {
                TempData["ErrorValoracion"] = "Debes comprar este producto antes de poder valorarlo.";
            }
            else
            {
                TempData["SuccessValoracion"] = "¡Gracias por tu reseña!";
            }

            return RedirectToAction("Details", new { idProducto = idProducto });
        }
    }
}
