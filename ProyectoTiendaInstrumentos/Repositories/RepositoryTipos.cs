using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Helpers;
using ProyectoTiendaInstrumentos.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryTipos
    {
        private ProyectoTiendaInstrumentosContext context;
        private HelperPathProvider helper;

        public RepositoryTipos(ProyectoTiendaInstrumentosContext context, HelperPathProvider helper)
        {
            this.context = context;
            this.helper = helper;   
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

        public async Task InsertFamiliaAsync(string nombre, IFormFile imagen)
        {
            Familia familia = new Familia();
            familia.Nombre = nombre;
            familia.IdFamilia = await this.context.Familias.MaxAsync(f => f.IdFamilia) + 1;

            if (imagen != null && imagen.Length > 0)
            {
                string fileName = imagen.FileName;
                string path = this.helper.MapPath(fileName, Folders.Familias);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
                familia.ImagenBanner = imagen.FileName;
            }
            else
            {
                familia.ImagenBanner = "default.png";
            }
            await this.context.Familias.AddAsync(familia);
            await this.context.SaveChangesAsync();
        }
        public async Task InsertTipoAsync(string nombre, int idFamilia)
        {
            Tipo tipo = new Tipo();
            tipo.Nombre = nombre;
            tipo.IdTipo = await this.context.Tipos.MaxAsync(f => f.IdTipo) + 1;
            tipo.IdFamilia = idFamilia;
            await this.context.Tipos.AddAsync(tipo);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteTipoAsync(int idTipo)
        {
            Tipo tipo = await this.context.Tipos.FirstOrDefaultAsync(f => f.IdTipo == idTipo);
            if (tipo != null)
            {
                this.context.Tipos.Remove(tipo);
                await this.context.SaveChangesAsync();
            }
        }
        public async Task DeleteFamiliaAsync(int idFamilia)
        {
            Familia familia = await this.context.Familias.FirstOrDefaultAsync(f => f.IdFamilia== idFamilia);
            if (familia != null)
            {
                this.context.Familias.Remove(familia);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
