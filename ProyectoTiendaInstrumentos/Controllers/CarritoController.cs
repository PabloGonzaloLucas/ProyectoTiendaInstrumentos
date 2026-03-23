using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;
using System.Security.Claims;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class CarritoController : Controller
    {
        private IRepositoryCarrito repo;
        public CarritoController(IRepositoryCarrito repositoryCarrito)
        {
            this.repo = repositoryCarrito;
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> Index(int pagina = 1)
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<VwCatalogoProducto> productosCarrito = await this.repo.GetProductosCarritoByUsuarioAsync(id);

                var paginado = PagedResult<VwCatalogoProducto>.Create(productosCarrito, pagina, 5);
                ViewBag.PaginacionCarrito = paginado;
                return View(paginado.Items);
            }
            catch
            {
                TempData["Error"] = "No se pudo cargar el carrito.";
                ViewBag.PaginacionCarrito = PagedResult<VwCatalogoProducto>.Create(new List<VwCatalogoProducto>(), 1, 5);
                return View(new List<VwCatalogoProducto>());
            }
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> ConfirmarCompra()
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<VwCatalogoProducto> productosCarrito = await this.repo.GetProductosCarritoByUsuarioAsync(id);
                return View(productosCarrito);
            }
            catch
            {
                TempData["Error"] = "No se pudo cargar la confirmación de compra.";
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(string accion, List<ProductoCarritoCantidad> productos)
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (accion == "cancelar")
                {
                    return RedirectToAction("Index");
                }
                else if (accion == "confirmar")
                {
                    if (productos == null || productos.Count == 0)
                    {
                        TempData["Error"] = "No se han recibido productos para confirmar la compra.";
                        return RedirectToAction("Index");
                    }

                    int idPedido = await this.repo.ComprarProductosAsync(productos, id);
                    if (idPedido == 0)
                    {
                        TempData["Error"] = "No se pudo procesar la compra. Por favor, inténtalo de nuevo.";
                        return RedirectToAction("Index");
                    }
                    await this.repo.ClearCartAsync(id);
                    return RedirectToAction("DetallesPedido", "Pedidos", new { idPedido = idPedido });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error procesando la compra.";
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarCompra(List<ProductoCarritoCantidad> productos)
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (productos == null || productos.Count == 0)
                {
                    TempData["Error"] = "El carrito está vacío";
                    return RedirectToAction("Index");
                }

                foreach (ProductoCarritoCantidad item in productos)
                {
                    if (item.Cantidad < 1)
                    {
                        TempData["Error"] = "Cantidad inválida.";
                        return RedirectToAction("Index");
                    }

                    await this.repo.ActualizarCantidadAsync(item.IdProducto, item.Cantidad, id);
                }

                return RedirectToAction("ConfirmarCompra");
            }
            catch
            {
                TempData["Error"] = "No se pudo procesar el carrito.";
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VaciarCarrito()
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                await this.repo.ClearCartAsync(id);
                TempData["Success"] = "Carrito vaciado";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "No se pudo vaciar el carrito.";
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int idProducto)
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (idProducto <= 0)
                {
                    TempData["Error"] = "Producto inválido.";
                    return RedirectToAction("Index");
                }

                await this.repo.RemoveProductoFromCartAsync(idProducto, id);
                TempData["Success"] = "Producto eliminado del carrito";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "No se pudo eliminar el producto del carrito.";
                return RedirectToAction("Index");
            }
        }

    }
}
