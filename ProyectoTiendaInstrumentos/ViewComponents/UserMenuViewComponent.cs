using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;

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
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Usuario no autenticado
                return View("NotAuthenticated");
            }

            // Usuario autenticado - obtener datos
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(userId));

            return View("Authenticated", usuario);
        }
    }
}