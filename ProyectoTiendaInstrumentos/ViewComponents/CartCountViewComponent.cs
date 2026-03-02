using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;

namespace ProyectoTiendaInstrumentos.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private IRepositoryCarrito repo;
        public CartCountViewComponent(IRepositoryCarrito repositoryCarrito)
        {
            this.repo = repositoryCarrito;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Obtener cantidad de items del carrito desde la sesión
            if(HttpContext.Session.GetObject<Usuario>("Usuario") == null)
            {
                return View(0);
            }
            int idUsuario = HttpContext.Session.GetObject<Usuario>("Usuario").IdUsuario;
            int cartCount = await this.repo.GetNumProductosCarritoAsync(idUsuario);
            return View(cartCount);
        }
    }
}