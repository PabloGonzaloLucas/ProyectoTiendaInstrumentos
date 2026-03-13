using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;
using System.Security.Claims;

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
        public async Task<IActionResult> Index(int idSubtipo, string q = null, int pagina = 1)
        {
            const int tamañoPagina = 12;

            if (!string.IsNullOrWhiteSpace(q))
            {
                List<VwCatalogoProducto> resultados = await this.repo.BuscarProductosPorNombreAsync(q);

                if (resultados.Count > 0)
                {
                    var subtiposEncontrados = resultados.Select(p => p.IdSubtipo).Distinct().ToList();
                    if (subtiposEncontrados.Count == 1)
                    {
                        int subtipoInferido = subtiposEncontrados[0];
                        ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(subtipoInferido);
                        ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(subtipoInferido);
                    }
                }

                var paginado = PagedResult<VwCatalogoProducto>.Create(resultados, pagina, tamañoPagina);
                ViewBag.TerminoBusqueda = q;
                ViewBag.NumeroResultados = resultados.Count;
                ViewBag.Paginacion = paginado;
                return View(paginado.Items);
            }

            ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
            List<Especificacion> specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);
            ViewBag.Specs = specs;

            var paginadoCatalogo = PagedResult<VwCatalogoProducto>.Create(productos, pagina, tamañoPagina);
            ViewBag.Paginacion = paginadoCatalogo;
            return View(paginadoCatalogo.Items);
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

        public async Task<IActionResult> Details(int idProducto, int paginaResenas = 1)
        {
            ViewBag.ImagenesProductos = await this.repo.GetImagenesProductoByIdAsync(idProducto);
            ViewBag.Especificaciones = await this.repo.GetEspecificacionesAsync(idProducto);

            List<VwValoracionesProducto> todasValoraciones = await this.repoValoraciones.GetValoracionesByProductoAsync(idProducto);
            var paginadoResenas = PagedResult<VwValoracionesProducto>.Create(todasValoraciones, paginaResenas, 5);
            ViewBag.Valoraciones = paginadoResenas.Items;
            ViewBag.PaginacionResenas = paginadoResenas;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        [AuthorizeUsuarios]
        public async Task<IActionResult> AddToCart(int idProducto, int cantidad = 1)
        {
            
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await this.repoCarrito.AddProductoToCartAsync(idProducto, idUsuario,cantidad);
            return RedirectToAction("Details", new { idProducto = idProducto });
            
            
        }

        [HttpPost]
        [AuthorizeUsuarios]

        public async Task<IActionResult> AddValoracion(int idProducto, int puntuacion, string comentario)
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));


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

        public async Task<IActionResult> ValoresEspecificacion(int idSubtipo, int idEspecificacion, string q = null)
        {
            ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
            ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);

            ViewBag.IdEspecificacionSeleccionada = idEspecificacion;
            ViewBag.ValoresEspecificacion = await this.repo.GetValoresEspecificacionesByEspecifiacionSubtipoAsync(idEspecificacion, idSubtipo);

            if (!string.IsNullOrWhiteSpace(q))
            {
                List<VwCatalogoProducto> resultados = await this.repo.BuscarProductosPorNombreAsync(q);
                ViewBag.TerminoBusqueda = q;
                ViewBag.NumeroResultados = resultados.Count;

                // Asegura que el menú de filtros aparezca también en modo búsqueda
                ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
                ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);

                return View("Index", resultados);
            }

            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
            return View("Index", productos);
        }

        [HttpGet]
        public async Task<IActionResult> ValoresEspecificacionPartial(int idSubtipo, int idEspecificacion)
        {
            List<ValoresEspecificacion> valores = await this.repo
                .GetValoresEspecificacionesByEspecifiacionSubtipoAsync(idEspecificacion, idSubtipo);

            return PartialView("_ValoresEspecificacion", valores);
        }

        [HttpGet]
        public async Task<IActionResult> TodosLosProductosPartial(int idSubtipo)
        {
            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
            return PartialView("_ProductosGrid", productos);
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarPorValor(int idSubtipo, int idEspecificacion, string valor, string? q = null)
        {
            ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
            ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);

            ViewBag.IdEspecificacionSeleccionada = idEspecificacion;
            ViewBag.ValoresEspecificacion = await this.repo.GetValoresEspecificacionesByEspecifiacionSubtipoAsync(idEspecificacion, idSubtipo);
            ViewBag.ValorSeleccionado = valor;

            if (!string.IsNullOrWhiteSpace(q))
            {
                List<VwCatalogoProducto> resultados = await this.repo.BuscarProductosPorNombreAsync(q);
                resultados = resultados
                    .Where(p => p.IdSubtipo == idSubtipo)
                    .ToList();

                return PartialView("_ProductosGrid", resultados);
            }

            List<VwCatalogoProducto> productos = await this.repo.GetVistaCatalogoBySubtipoAndFiltroAsync(idSubtipo, idEspecificacion, valor);
            return PartialView("_ProductosGrid", productos);
        }
    }
}
