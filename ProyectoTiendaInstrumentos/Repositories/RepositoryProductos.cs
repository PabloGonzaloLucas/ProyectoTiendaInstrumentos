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
        public async Task<Producto> GetProductoAsync(int idProducto)
        {
            var consulta = from datos in this.context.Productos
                           where datos.IdProducto == idProducto
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<VwDetallesProducto> GetDetallesProductoAsync(int idProducto)
        {
            var consulta = from datos in this.context.DetallesProductos
                           where datos.IdProducto == idProducto
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<List<VwEspecificacionesProducto>> GetEspecificacionesAsync(int idProducto)
        {
            var consulta = from datos in this.context.EspecificacionesProducto
                           where datos.IdProducto == idProducto
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoAsync()
        {
            var consulta = from datos in this.context.CatalogoProductos
                           select datos;
            List<VwCatalogoProducto> catalogo = await consulta.ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
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
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
            }
            return catalogo;
        }
        public async Task<List<ProductoImagen>> GetImagenesProductoByIdAsync(int idProducto)
        {
            var consulta = from datos in this.context.ProductosImagenes
                           where datos.IdProducto == idProducto
                           select datos;
            
            return await consulta.ToListAsync();
        }
        public async Task<Subtipo> GetSubtipoByIdAsync(int idSubtipo)
        {
            var consulta = from datos in this.context.Subtipos
                           where datos.IdSubtipo == idSubtipo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
    }
}
