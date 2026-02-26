using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryTipos
    {
        private ProyectoTiendaInstrumentosContext context;
        public RepositoryTipos(ProyectoTiendaInstrumentosContext context)
        {
            this.context = context;
        }

        public async Task<List<Tipo>> GetTiposAsync()
        {
            var consulta = from datos in this.context.Tipos
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<Tipo>> GetTiposByFamiliaAsync(int idFamilia)
        {
            var consulta = from datos in this.context.Tipos
                           where datos.IdFamilia == idFamilia
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
