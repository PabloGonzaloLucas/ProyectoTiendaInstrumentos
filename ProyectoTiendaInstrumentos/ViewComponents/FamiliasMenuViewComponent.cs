using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;

namespace ProyectoTiendaInstrumentos.ViewComponents
{
    public class FamiliasMenuViewComponent : ViewComponent
    {
        private readonly ProyectoTiendaInstrumentosContext _context;

        public FamiliasMenuViewComponent(ProyectoTiendaInstrumentosContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var familias = await _context.Familias.ToListAsync();
            return View(familias);
        }
    }
}