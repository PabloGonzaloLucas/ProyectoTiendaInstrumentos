using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Filters;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories;
using System.Security.Claims;

namespace ProyectoTiendaInstrumentos.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly ProyectoTiendaInstrumentosContext _context;
        private RepositoryUser repo;
        public UserMenuViewComponent(ProyectoTiendaInstrumentosContext context, RepositoryUser repositoryUser)
        {
            _context = context;
            this.repo = repositoryUser;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) == null) {
                return View("NotAuthenticated");
            }
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario user = await this.repo.GetUserByIdAsync(idUsuario);
            if (user == null)
            {
                // Usuario no autenticado
                return View("NotAuthenticated");
            }


            return View("Authenticated", user);
        }
    }
}