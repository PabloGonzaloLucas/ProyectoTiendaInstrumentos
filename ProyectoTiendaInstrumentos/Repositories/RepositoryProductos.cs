using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryProductos
    {
        private ProyectoTiendaInstrumentosContext context;
        public RepositoryProductos(ProyectoTiendaInstrumentosContext context)
        {
            this.context = context;
        }

        public async Task<List<Producto>> GetProductosAsync()
        {
            var consulta = from datos in this.context.Productos
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<string>> GetNombresEspecificacionesAsync(int idProducto)
        {
            var consulta = from datos in this.context.EspecificacionesProducto
                           where datos.IdProducto == idProducto
                           select datos.Especificacion;
            return await consulta.ToListAsync();
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoAsync()
        {
            var consulta = from datos in this.context.CatalogoProductos
                           select datos;
            List<VwCatalogoProducto> catalogo = await consulta.ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetNombresEspecificacionesAsync(producto.IdProducto);
            }
            return catalogo;
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoBySubtipoAsync(int idSubtipo)
        {
            var consulta = from datos in this.context.CatalogoProductos
                           where datos.IdSubtipo == idSubtipo
                           select datos;
            List<VwCatalogoProducto> catalogo = await consulta.ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetNombresEspecificacionesAsync(producto.IdProducto);
            }
            return catalogo;
        }
    }
}
