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
        public async Task<IActionResult> Index(int idSubtipo, int? idMarca = null, string q = null, int pagina = 1, string? todos = "false", string? orden = "")
        {

            const int tamañoPagina = 12;

            // --- ordenar por ventas (Top 100) ---
            if (string.Equals(orden, "ventas", StringComparison.OrdinalIgnoreCase))
            {
                if (idSubtipo > 0)
                {
                    ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
                    ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);
                }

                List<VwCatalogoProducto> top = await this.repo.GetVistaCatalogoAsync();

                if (idSubtipo > 0)
                {
                    top = top.Where(p => p.IdSubtipo == idSubtipo).ToList();
                }

                top = top
                    .OrderByDescending(p => p.Ventas ?? 0)
                    .ThenBy(p => p.Modelo)
                    .Take(100)
                    .ToList();

                var paginadoTop = PagedResult<VwCatalogoProducto>.Create(top, pagina, tamañoPagina);
                ViewBag.TerminoBusqueda = "Top ventas";
                ViewBag.NumeroResultados = top.Count;
                ViewBag.Paginacion = paginadoTop;
                return View(paginadoTop.Items);
            }

            // --- ver todos (catálogo completo) ---
            // Mantiene la barra de filtros (familia/tipo/subtipo) y el panel de especificaciones.
            // Si idSubtipo > 0, seguimos cargando el subtipo y sus specs para que el panel funcione.
            if (string.Equals(todos, "true", StringComparison.OrdinalIgnoreCase))
            {
                if (idSubtipo > 0)
                {
                    ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
                    ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);
                }

                List<VwCatalogoProducto> productosTodos = await this.repo.GetVistaCatalogoAsync();

                // Si venimos con idSubtipo, opcionalmente filtramos el grid al subtipo.
                // La UI puede seguir mostrando el panel de specs de ese subtipo.
                if (idSubtipo > 0)
                {
                    productosTodos = productosTodos.Where(p => p.IdSubtipo == idSubtipo).ToList();
                }

                var paginadoTodos = PagedResult<VwCatalogoProducto>.Create(productosTodos, pagina, tamañoPagina);
                ViewBag.TerminoBusqueda = "Todos";
                ViewBag.NumeroResultados = productosTodos.Count;
                ViewBag.Paginacion = paginadoTodos;
                return View(paginadoTodos.Items);
            }

            // --- filtro por marca ---
            if (idMarca.HasValue)
            {
                // Resolvemos el nombre de la marca desde BD (vw_CatalogoProductos expone "Marca", no "IdMarca").
                var marcas = await this.repo.GetMarcasAsync();
                var marca = marcas.FirstOrDefault(m => m.IdMarca == idMarca.Value);
                if (marca == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                List<VwCatalogoProducto> baseCatalogo;
                if (idSubtipo > 0)
                {
                    ViewBag.Subtipo = await this.repo.GetSubtipoByIdAsync(idSubtipo);
                    ViewBag.Specs = await this.repo.GetEspecificacionesBySubtipoAsync(idSubtipo);
                    baseCatalogo = await this.repo.GetVistaCatalogoBySubtipoAsync(idSubtipo);
                }
                else
                {
                    baseCatalogo = await this.repo.GetVistaCatalogoAsync();
                }

                var filtrados = baseCatalogo
                    .Where(p => string.Equals(p.Marca, marca.Nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var paginadoMarca = PagedResult<VwCatalogoProducto>.Create(filtrados, pagina, tamañoPagina);
                ViewBag.TerminoBusqueda = marca.Nombre;
                ViewBag.NumeroResultados = filtrados.Count;
                ViewBag.Paginacion = paginadoMarca;
                return View(paginadoMarca.Items);
            }

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
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddToCart(int idProducto, int cantidad = 1)
        {

            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await this.repoCarrito.AddProductoToCartAsync(idProducto, idUsuario, cantidad);
            return RedirectToAction("Details", new { idProducto = idProducto });


        }

        [HttpPost]
        [AuthorizeUsuarios]
        [ValidateAntiForgeryToken]

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

        [AuthorizeUsuarios(Policy = "AdminOnly")]

        public async Task<IActionResult> Eliminar(int idProducto, int? idSubtipo)
        {
            TempData["Error"] = "Operación no permitida por GET. Confirma la eliminación desde el botón.";
            if (idSubtipo.HasValue)
            {
                return RedirectToAction("Index", "Productos", new { idSubtipo = idSubtipo.Value });
            }
            return RedirectToAction("Details", "Productos", new { idProducto });
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPost(int idProducto, int? idSubtipo)
        {
            try
            {
                if (idProducto <= 0)
                {
                    TempData["Error"] = "Identificador de producto inválido.";
                    return RedirectToAction("Index", "Home");
                }

                if (!await this.repo.TieneComprasAsync(idProducto))
                {
                    await this.repo.DeleteProductoAsync(idProducto);
                    TempData["Success"] = "Producto eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se puede eliminar un producto con compras asociadas.";
                    return RedirectToAction("Details", "Productos", new { idProducto = idProducto });
                }

                if (idSubtipo.HasValue)
                {
                    return RedirectToAction("Index", "Productos", new { idSubtipo = idSubtipo.Value });
                }

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["Error"] = "No se pudo eliminar el producto.";
                return RedirectToAction("Details", "Productos", new { idProducto = idProducto });
            }
        }
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(int? idSubtipo)
        {
            if (idSubtipo != null)
            {
                SelectSubtiposProducto selectSubtiposProducto = await this.repo.GetArbolTiposAsync(idSubtipo.Value);
                ViewBag.ArbolTipos = selectSubtiposProducto;
            }
            ViewBag.Marcas = await this.repo.GetMarcasAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(
            string modelo,
            int idMarca,
            decimal precio,
            int stock,
            int idSubtipo,
            string descripcion,
            IFormFile imagen,
            List<IFormFile> imagenes,
            List<int>? specIds,
            List<string>? specValores)
        {
            int idProductoNuevo = await this.repo.InsertProductoAsync(
                modelo: modelo,
                idMarca: idMarca,
                precio: precio,
                stock: stock,
                idSubtipo: idSubtipo,
                descripcion: descripcion,
                imagenPrincipal: imagen,
                imagenesSecundarias: imagenes,
                specIds: specIds,
                specValores: specValores);

            return RedirectToAction("Details", "Productos", new { idProducto = idProductoNuevo });
        }
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> Editar(int idProducto)
        {
            Producto producto = await this.repo.GetProductoAsync(idProducto);
            if (producto == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (producto.IdSubtipo.HasValue)
            {
                ViewBag.ArbolTipos = await this.repo.GetArbolTiposAsync(producto.IdSubtipo.Value);
            }

            ViewBag.Marcas = await this.repo.GetMarcasAsync();
            ViewBag.ImagenesProducto = await this.repo.GetImagenesProductoByIdAsync(idProducto);
            ViewBag.Producto = producto;
            ViewBag.Especificaciones = await this.repo.GetEspecificacionesAsync(idProducto);
            return View();
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int idProducto,
            string modelo,
            int idMarca,
            decimal precio,
            int stock,
            int idSubtipo,
            string descripcion,
            IFormFile? imagen,
            List<IFormFile>? imagenes,
            List<int>? imagenesExistentesIds,
            List<int>? specIds,
            List<string>? specValores)
        {
            await this.repo.UpdateProductoAsync(
                idProducto: idProducto,
                modelo: modelo,
                idMarca: idMarca,
                precio: precio,
                stock: stock,
                idSubtipo: idSubtipo,
                descripcion: descripcion,
                imagenPrincipal: imagen,
                imagenesSecundarias: imagenes,
                imagenesExistentesIds: imagenesExistentesIds,
                specIds: specIds,
                specValores: specValores);

            return RedirectToAction("Details", "Productos", new { idProducto });
        }
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateSpecs(int? idSubtipo)
        {
            if (idSubtipo != null)
            {
                SelectSubtiposProducto selectSubtiposProducto = await this.repo.GetArbolTiposAsync(idSubtipo.Value);
                ViewBag.ArbolTipos = selectSubtiposProducto;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateSpecs(string nombre, string unidadMedida, int idSubtipo)
        {
            await this.repo.InsertEspecificacionAsync(
                nombre: nombre,
                unidadMedida: unidadMedida,
                idSubtipo: idSubtipo);

            return RedirectToAction("Index", "Productos", new { idSubtipo });
        }
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> EditSpec(int? idSubtipo = null)
        {
            if (idSubtipo != null)
            {
                ViewBag.ArbolTipos = await this.repo.GetArbolTiposAsync(idSubtipo.Value);
            }

            // Pantalla dinámica: no cargamos ninguna especificación por defecto.
            // La UI cargará las especificaciones por subtipo y, al seleccionar una,
            // pedirá sus datos vía JSON.
            return View();
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetEspecificacionJson(int idEspecificacion)
        {
            Especificacion? espec = await this.repo.GetEspecificacionByIdAsync(idEspecificacion);
            if (espec == null)
            {
                return NotFound();
            }

            return Json(new
            {
                idEspecificacion = espec.IdEspecificacion,
                nombre = espec.Nombre,
                unidadMedida = espec.UnidadMedida
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> EditSpec(int idEspecificacion, string nombre, string unidadMedida, int idSubtipo)
        {
            await this.repo.UpdateEspecificacionAsync(
                idEspecificacion: idEspecificacion,
                nombre: nombre,
                unidadMedida: unidadMedida,
                idSubtipo: idSubtipo);

            return RedirectToAction("Index", "Productos", new { idSubtipo });
        }

        public async Task<IActionResult> Marcas()
        {
            List<Marca> marcas = await this.repo.GetMarcasAsync();
            return View(marcas);
        }


        [HttpPost]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateMarca(string nombre, IFormFile logo)
        {
            await this.repo.InsertMarcaAsync(nombre, logo);

            return RedirectToAction("Marcas", "Productos");
        }


        [AuthorizeUsuarios(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> EditMarca(int idMarca)
        {

            Marca marca = await this.repo.GetMarcaAsync(idMarca);
            // Pantalla dinámica: no cargamos ninguna especificación por defecto.
            // La UI cargará las especificaciones por subtipo y, al seleccionar una,
            // pedirá sus datos vía JSON.
            return View(marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> EditMarca(int idMarca, string nombre, IFormFile? logo)
        {
            await this.repo.UpdateMarcaAsync(idMarca, nombre, logo);
            return RedirectToAction("Marcas", "Productos");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteMarca(int idMarca)
        {
            if (!await this.repo.TieneProductosAsync(idMarca))
            {
                await this.repo.DeleteMarcaAsync(idMarca);
            }
            else
            {
                TempData["ErrorMarca"] = "No se puede eliminar esta marca porque tiene productos asociados.";
            }
            return RedirectToAction("Marcas", "Productos");
        }
    }
}
