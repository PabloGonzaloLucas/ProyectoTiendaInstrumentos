using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly ProyectoTiendaInstrumentosContext _context;

        public UserMenuViewComponent(ProyectoTiendaInstrumentosContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.Session.GetObject<Usuario>("Usuario");

            if (user == null)
            {
                // Usuario no autenticado
                return View("NotAuthenticated");
            }


            return View("Authenticated", user);
        }
    }
}