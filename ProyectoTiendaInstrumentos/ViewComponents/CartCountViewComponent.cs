using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;
using System.Security.Claims;

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
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            int cartCount = await this.repo.GetNumProductosCarritoAsync(idUsuario);
            return View(cartCount);
        }
    }
}