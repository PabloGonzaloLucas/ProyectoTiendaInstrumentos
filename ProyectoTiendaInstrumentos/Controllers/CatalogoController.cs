using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Repositories;

namespace ProyectoTiendaInstrumentos.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ProyectoTiendaInstrumentosContext context;
        private readonly RepositoryTipos repoTipos;
        private readonly RepositorySubtipos repoSubtipos;

        public CatalogoController(
            ProyectoTiendaInstrumentosContext context,
            RepositoryTipos repoTipos,
            RepositorySubtipos repoSubtipos)
        {
            this.context = context;
            this.repoTipos = repoTipos;
            this.repoSubtipos = repoSubtipos;
        }

        [HttpGet]
        public async Task<IActionResult> Familias()
        {
            var familias = await this.context.Familias
                .Select(f => new { id = f.IdFamilia, nombre = f.Nombre })
                .ToListAsync();

            return Json(familias);
        }

        [HttpGet]
        public async Task<IActionResult> Tipos(int idFamilia)
        {
            var tipos = await this.repoTipos.GetTiposByFamiliaAsync(idFamilia);
            var result = tipos.Select(t => new { id = t.IdTipo, nombre = t.Nombre }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Subtipos(int idTipo)
        {
            var subtipos = await this.repoSubtipos.GetSubtiposByTipoAsync(idTipo);
            var result = subtipos.Select(s => new { id = s.IdSubtipo, nombre = s.Nombre }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Especificaciones(int idSubtipo)
        {
            var specs = await (
                    from cf in this.context.CategoriaFiltros
                    join e in this.context.Especificaciones on cf.IdEspecificacion equals e.IdEspecificacion
                    where cf.IdSubtipo == idSubtipo
                    orderby e.Nombre
                    select new { id = e.IdEspecificacion, nombre = e.Nombre, unidadMedida = e.UnidadMedida }
                )
                .ToListAsync();

            return Json(specs);
        }
    }
}