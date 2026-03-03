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
        public async Task<IActionResult> MisPedidos()
        {

            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            List<Pedido> pedidos = await this.repo.GetPedidosByUsuarioAsync(idUsuario);

            return View(pedidos);
        }

        public async Task<IActionResult> DetallesPedido(int idPedido)
        {
            if (HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            Pedido pedido = await this.repo.GetPedidoByIdAsync(idPedido);

            return View(pedido);
        }
    }
}
