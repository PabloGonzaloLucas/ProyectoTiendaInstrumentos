using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Extensions;
using ProyectoTiendaInstrumentos.Models;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryCarritoSession : IRepositoryCarrito
    {
        private IHttpContextAccessor contextAccessor;
        private ProyectoTiendaInstrumentosContext context;

        public RepositoryCarritoSession(IHttpContextAccessor contextAccessor, ProyectoTiendaInstrumentosContext context)
        {
            this.contextAccessor = contextAccessor;
            this.context = context;
        }
        public async Task AddProductoToCartAsync(int idProducto, int idUsuario, int cantidad)
        {
            List<ProductoCarritoCantidad> idsProductos;
            if (this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") != null)
            {
                idsProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito");
            }
            else
            {
                idsProductos = new List<ProductoCarritoCantidad>();
            }
            idsProductos.Add(new ProductoCarritoCantidad { IdProducto = idProducto, Cantidad = cantidad});
            this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", idsProductos);
        }

        public async Task ClearCartAsync(int idUsuario)
        {
            this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", null);
        }

        public async Task<List<VwCatalogoProducto>> GetProductosCarritoByUsuarioAsync(int idUsuario)
        {
            if(this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") == null)
            {
                return new List<VwCatalogoProducto>();
            }
            else
            {
                List<ProductoCarritoCantidad> idsProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito");
                List<int> productosIds = idsProductos.Select(p => p.IdProducto).ToList();
                var consulta = from datos in this.context.CatalogoProductos
                               where productosIds.Contains(datos.IdProducto)
                               select datos;
                List<VwCatalogoProducto> productos =  await consulta.ToListAsync();
                foreach(VwCatalogoProducto producto in productos)
                {
                    producto.Cantidad = idsProductos.Where(p => p.IdProducto == producto.IdProducto).FirstOrDefault().Cantidad;
                }
                return productos;
            }

        }

        public async Task RemoveProductoFromCartAsync(int idProducto, int idUsuario)
        {
            if(this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") != null)
            {
                List<ProductoCarritoCantidad> idsProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito");
                ProductoCarritoCantidad producto = idsProductos.Where(p => p.IdProducto == idProducto).FirstOrDefault();
                idsProductos.Remove(producto);
                this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", idsProductos);
            }
        }

        public async Task<int> GetNumProductosCarritoAsync(int idUsuario)
        {
            if(this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") == null)
            {
                return 0;
            }
            int numProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito").Count;
            return numProductos;
        }

        public async Task ActualizarCantidadAsync(int idProducto, int cantidad, int idUsuario)
        {
            if(this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") != null)
            {
                List<ProductoCarritoCantidad> idsProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito");
                ProductoCarritoCantidad producto = idsProductos.Where(p => p.IdProducto == idProducto).FirstOrDefault();
                if(producto != null)
                {
                    producto.Cantidad = cantidad;
                    this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", idsProductos);
                }
            }
        }
    }
}
