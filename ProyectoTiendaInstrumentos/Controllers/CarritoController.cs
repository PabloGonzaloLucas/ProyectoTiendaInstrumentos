using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class CarritoController : Controller
    {
        private IRepositoryCarrito repo;
        public CarritoController(IRepositoryCarrito repositoryCarrito)
        {
            this.repo = repositoryCarrito;
        }
        public async Task<IActionResult> Index()
        {
            if(HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            List<VwCatalogoProducto> productosCarrito = await this.repo.GetProductosCarritoByUsuarioAsync(idUsuario);
            return View(productosCarrito);
        }
        public async Task<IActionResult> ConfirmarCompra()
        {
            if(HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            List<VwCatalogoProducto> productosCarrito = await this.repo.GetProductosCarritoByUsuarioAsync(idUsuario);
            return View(productosCarrito);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra(string accion, List<ProductoCarritoCantidad> productos)
        {
            if(HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;

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

                int idPedido = await this.repo.ComprarProductosAsync(productos, idUsuario);
                if(idPedido == 0)
                {
                    TempData["Error"] = "No se pudo procesar la compra. Por favor, inténtalo de nuevo.";
                    return RedirectToAction("Index");
                }
                await this.repo.ClearCartAsync(idUsuario);
                //REDIRIJIR AL PEDIDO
                return RedirectToAction("DetallesPedido", "Pedidos", new { idPedido = idPedido });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarCompra(List<ProductoCarritoCantidad> productos)
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;

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
                await this.repo.ActualizarCantidadAsync(item.IdProducto, item.Cantidad, idUsuario);
            }

            return RedirectToAction("ConfirmarCompra");
        
        }

        [HttpPost]
        public async Task<IActionResult> VaciarCarrito()
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            else
            {
                int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
                await this.repo.ClearCartAsync(idUsuario);
                TempData["Mensaje"] = "Carrito vaciado";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int idProducto)
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            else
            {
                int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
                await this.repo.RemoveProductoFromCartAsync(idProducto, idUsuario);
                TempData["Mensaje"] = "Producto eliminado del carrito";
                return RedirectToAction("Index");
            }

        }

    }
}
