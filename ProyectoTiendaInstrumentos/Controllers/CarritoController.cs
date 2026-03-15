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
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<VwCatalogoProducto> productosCarrito = await this.repo.GetProductosCarritoByUsuarioAsync(id);

            var paginado = PagedResult<VwCatalogoProducto>.Create(productosCarrito, pagina, 5);
            ViewBag.PaginacionCarrito = paginado;
            return View(paginado.Items);
        }
        [AuthorizeUsuarios]

        public async Task<IActionResult> ConfirmarCompra()
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<VwCatalogoProducto> productosCarrito = await this.repo.GetProductosCarritoByUsuarioAsync(id);
            return View(productosCarrito);
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(string accion, List<ProductoCarritoCantidad> productos)
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
                if(idPedido == 0)
                {
                    TempData["Error"] = "No se pudo procesar la compra. Por favor, inténtalo de nuevo.";
                    return RedirectToAction("Index");
                }
                await this.repo.ClearCartAsync(id);
                //REDIRIJIR AL PEDIDO
                return RedirectToAction("DetallesPedido", "Pedidos", new { idPedido = idPedido });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ProcesarCompra(List<ProductoCarritoCantidad> productos)
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (productos == null || productos.Count == 0)
            {
                TempData["Error"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            // Aquí procesarías la compra con las cantidades
            // Por ejemplo: crear pedido, reducir stock, etc.

            // TODO: Implementar lógica de compra
            
            foreach (ProductoCarritoCantidad item in productos)
            {
                await this.repo.ActualizarCantidadAsync(item.IdProducto, item.Cantidad, id);
            }

            return RedirectToAction("ConfirmarCompra");
        
        }

        [AuthorizeUsuarios]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> VaciarCarrito()
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await this.repo.ClearCartAsync(id);
            TempData["Mensaje"] = "Carrito vaciado";
            return RedirectToAction("Index");
            
        }

        [AuthorizeUsuarios]

        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int idProducto)
        {
            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await this.repo.RemoveProductoFromCartAsync(idProducto, id);
            TempData["Mensaje"] = "Producto eliminado del carrito";
            return RedirectToAction("Index");
            

        }

    }
}
