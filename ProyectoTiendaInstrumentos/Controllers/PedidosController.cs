using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using System.Security.Claims;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class PedidosController : Controller
    {
        private RepositoryPedidos repo;
        public PedidosController(RepositoryPedidos repo)
        {
            this.repo = repo;
        }

        [AuthorizeUsuarios]
        public IActionResult Index()
        {
            return View();

        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> MisPedidos(int pagina = 1)
        {
            try
            {
                int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<Pedido> pedidos = await this.repo.GetPedidosByUsuarioAsync(id);

                var paginado = PagedResult<Pedido>.Create(pedidos, pagina, 8);
                ViewBag.Paginacion = paginado;
                return View(paginado.Items);
            }
            catch
            {
                TempData["Error"] = "No se pudieron cargar tus pedidos.";
                ViewBag.Paginacion = PagedResult<Pedido>.Create(new List<Pedido>(), 1, 8);
                return View(new List<Pedido>());
            }
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> DetallesPedido(int idPedido, int pagina = 1)
        {
            try
            {
                if (idPedido <= 0)
                {
                    TempData["Error"] = "Identificador de pedido inválido.";
                    return RedirectToAction("MisPedidos");
                }

                Pedido pedido = await this.repo.GetPedidoByIdAsync(idPedido);
                if (pedido == null)
                {
                    TempData["Error"] = "No se encontró el pedido.";
                    return RedirectToAction("MisPedidos");
                }

                var productos = pedido.ProductosPedido ?? new List<VwProductosPedido>();
                var paginado = PagedResult<VwProductosPedido>.Create(productos, pagina, 5);
                ViewBag.PaginacionProductos = paginado;
                pedido.ProductosPedido = paginado.Items;

                return View(pedido);
            }
            catch
            {
                TempData["Error"] = "No se pudo cargar el detalle del pedido.";
                return RedirectToAction("MisPedidos");
            }
        }
    }
}
