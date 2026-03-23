using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Repositories;

namespace ProyectoTiendaInstrumentos.ViewComponents
{
    public class FamiliasMenuViewComponent : ViewComponent
    {
        private readonly ProyectoTiendaInstrumentosContext _context;
        private readonly RepositoryTipos repo;

        public FamiliasMenuViewComponent(ProyectoTiendaInstrumentosContext context, RepositoryTipos repo)
        {
            _context = context;
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var familias = await _context.Familias.ToListAsync();
                return View(familias);
            }
            catch
            {
                // Evita romper el layout si falla la BD.
                return View(Enumerable.Empty<ProyectoTiendaInstrumentos.Models.Familia>());
            }
        }
    }
}