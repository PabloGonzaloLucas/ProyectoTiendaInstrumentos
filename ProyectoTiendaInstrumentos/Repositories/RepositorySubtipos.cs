using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositorySubtipos
    {
        private ProyectoTiendaInstrumentosContext context;
        public RepositorySubtipos(ProyectoTiendaInstrumentosContext context)
        {
            this.context = context;
        }

        public async Task<List<Subtipo>> GetSubtiposAsync()
        {
            var consulta = from datos in this.context.Subtipos
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<Subtipo>> GetSubtiposByTipoAsync(int idTipo)
        {
            var consulta = from datos in this.context.Subtipos
                           where datos.IdTipo == idTipo
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Tipo> GetTipoByIdAsync(int idTipo)
        {
            var consulta = from datos in this.context.Tipos
                           where datos.IdTipo == idTipo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<string> GetNombreTipoByIdAsync(int idTipo)
        {
            var consulta = from datos in this.context.Tipos
                           where datos.IdTipo == idTipo
                           select datos.Nombre;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task InsertSubtipoAsync(string nombre, int idTipo)
        {
            Subtipo subtipo = new Subtipo();
            subtipo.Nombre = nombre;
            subtipo.IdSubtipo = await this.context.Subtipos.MaxAsync(f => f.IdSubtipo) + 1;
            subtipo.IdTipo = idTipo;
            await this.context.Subtipos.AddAsync(subtipo);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteSubtipoAsync(int idSubtipo)
        {
            Subtipo subtipo = await this.context.Subtipos.FirstOrDefaultAsync(f => f.IdSubtipo == idSubtipo);
            if (subtipo != null)
            {
                this.context.Subtipos.Remove(subtipo);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
