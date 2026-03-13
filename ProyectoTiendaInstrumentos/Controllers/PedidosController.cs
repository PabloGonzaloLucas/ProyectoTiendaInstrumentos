using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using System;
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

            int id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
           
            List<Pedido> pedidos = await this.repo.GetPedidosByUsuarioAsync(id);

            var paginado = PagedResult<Pedido>.Create(pedidos, pagina, 8);
            ViewBag.Paginacion = paginado;
            return View(paginado.Items);
        }
        [AuthorizeUsuarios]

        public async Task<IActionResult> DetallesPedido(int idPedido, int pagina = 1)
        {

            Pedido pedido = await this.repo.GetPedidoByIdAsync(idPedido);

            var paginado = PagedResult<VwProductosPedido>.Create(pedido.ProductosPedido, pagina, 5);
            ViewBag.PaginacionProductos = paginado;
            pedido.ProductosPedido = paginado.Items;

            return View(pedido);
        }
    }
}
