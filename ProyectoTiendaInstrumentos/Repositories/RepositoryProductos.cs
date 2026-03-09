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
        public async Task<List<VwCatalogoProducto>> BuscarProductosPorNombreAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return new List<VwCatalogoProducto>();
            }

            var consulta = from datos in this.context.CatalogoProductos
                           where datos.Modelo.Contains(termino) 
                              || datos.Marca.Contains(termino)
                              || datos.Tipo.Contains(termino)
                              || datos.Subtipo.Contains(termino)
                              || datos.Familia.Contains(termino)
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

        public async Task<List<Especificacion>> GetEspecificacionesBySubtipoAsync(int idSubtipo)
        {
            var consulta = from datos in this.context.Especificaciones
                                         join categoriaFiltro in this.context.CategoriaFiltros on datos.IdEspecificacion equals categoriaFiltro.IdEspecificacion
                                         where categoriaFiltro.IdSubtipo == idSubtipo 
                                         select datos;

            List<Especificacion> specs =await consulta.ToListAsync();
            return specs;
        }

        public async Task<List<ValoresEspecificacion>> GetValoresEspecificacionesByEspecifiacionAsync(int idEspecificacion)
        {
            var consulta = from datos in this.context.ValoresEspecifiaciones
                           where datos.IdEspecificacion == idEspecificacion
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<ValoresEspecificacion>> GetValoresEspecificacionesByEspecifiacionSubtipoAsync(int idEspecificacion, int idSubtipo)
        {
            var valores = await (
                from datos in this.context.ValoresEspecifiaciones
                join categoriaFiltro in this.context.CategoriaFiltros
                    on datos.IdEspecificacion equals categoriaFiltro.IdEspecificacion
                where datos.IdEspecificacion == idEspecificacion
                   && categoriaFiltro.IdSubtipo == idSubtipo
                group datos by datos.Valor into g
                select g.First()
            ).ToListAsync();

            return valores;
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoBySubtipoAndFiltroAsync(int idSubtipo, int idEspecificacion, string valor)
        {
            var consulta =
                from producto in this.context.CatalogoProductos
                join spec in this.context.EspecificacionesProducto on producto.IdProducto equals spec.IdProducto
                where producto.IdSubtipo == idSubtipo
                   && spec.IdEspecificacion == idEspecificacion
                   && spec.Valor == valor
                select producto;

            List<VwCatalogoProducto> catalogo = await consulta.Distinct().ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
            }

            return catalogo;
        }

    }
}
