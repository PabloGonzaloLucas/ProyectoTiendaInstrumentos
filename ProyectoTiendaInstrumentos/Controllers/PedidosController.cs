using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using System;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class PedidosController : Controller
    {
        private RepositoryPedidos repo;
        public PedidosController(RepositoryPedidos repo)
        {
            this.repo = repo;
        }
        public IActionResult Index()
        {
            return View();

        }
        public async Task<IActionResult> MisPedidos(int pagina = 1)
        {

            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            List<Pedido> pedidos = await this.repo.GetPedidosByUsuarioAsync(idUsuario);

            var paginado = PagedResult<Pedido>.Create(pedidos, pagina, 8);
            ViewBag.Paginacion = paginado;
            return View(paginado.Items);
        }

        public async Task<IActionResult> DetallesPedido(int idPedido, int pagina = 1)
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            Pedido pedido = await this.repo.GetPedidoByIdAsync(idPedido);

            var paginado = PagedResult<VwProductosPedido>.Create(pedido.ProductosPedido, pagina, 5);
            ViewBag.PaginacionProductos = paginado;
            pedido.ProductosPedido = paginado.Items;

            return View(pedido);
        }
    }
}
