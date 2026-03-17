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
            idsProductos.Add(new ProductoCarritoCantidad { IdProducto = idProducto, Cantidad = cantidad });
            this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", idsProductos);
        }

        public async Task ClearCartAsync(int idUsuario)
        {
            this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", null);
        }

        public async Task<List<VwCatalogoProducto>> GetProductosCarritoByUsuarioAsync(int idUsuario)
        {
            if (this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") == null)
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
                List<VwCatalogoProducto> productos = await consulta.ToListAsync();
                foreach (VwCatalogoProducto producto in productos)
                {
                    producto.Cantidad = idsProductos.Where(p => p.IdProducto == producto.IdProducto).FirstOrDefault().Cantidad;
                }
                return productos;
            }

        }

        public async Task RemoveProductoFromCartAsync(int idProducto, int idUsuario)
        {
            if (this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") != null)
            {
                List<ProductoCarritoCantidad> idsProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito");
                ProductoCarritoCantidad producto = idsProductos.Where(p => p.IdProducto == idProducto).FirstOrDefault();
                idsProductos.Remove(producto);
                this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", idsProductos);
            }
        }

        public async Task<int> GetNumProductosCarritoAsync(int idUsuario)
        {
            if (this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") == null)
            {
                return 0;
            }
            int numProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito").Count;
            return numProductos;
        }

        public async Task ActualizarCantidadAsync(int idProducto, int cantidad, int idUsuario)
        {
            if (this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito") != null)
            {
                List<ProductoCarritoCantidad> idsProductos = this.contextAccessor.HttpContext.Session.GetObject<List<ProductoCarritoCantidad>>("idsCarrito");
                ProductoCarritoCantidad producto = idsProductos.Where(p => p.IdProducto == idProducto).FirstOrDefault();
                if (producto != null)
                {
                    producto.Cantidad = cantidad;
                    this.contextAccessor.HttpContext.Session.SetObject("idsCarrito", idsProductos);
                }
            }
        }

        public async Task<int> ComprarProductosAsync(List<ProductoCarritoCantidad> productos, int idUsuario)
        {
            if (productos == null || productos.Count == 0)
            {
                return 0;
            }

            List<ProductoCarritoCantidad> productosValidos = productos
                .Where(p => p != null && p.IdProducto > 0 && p.Cantidad > 0)
                .ToList();

            if (productosValidos.Count == 0)
            {
                return 0;
            }

            await using var transaction = await this.context.Database.BeginTransactionAsync();

            Pedido pedido = new Pedido();

            foreach (ProductoCarritoCantidad producto in productosValidos)
            {
                Producto? productoDb = await this.context.Productos
                    .FirstOrDefaultAsync(p => p.IdProducto == producto.IdProducto);

                if (productoDb == null)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }

                int stockProducto = productoDb.Stock ?? 0;
                if (stockProducto < producto.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }

                pedido.PrecioTotal += productoDb.Precio * producto.Cantidad;
            }

            int nextPedidoId = await this.FindMaxIdPedidoAsync() + 1;
            pedido.IdPedido = nextPedidoId;
            pedido.FechaCreacion = DateTime.Now;
            pedido.IdUsuario = idUsuario;
            pedido.Estado = "Procesando";

            // FechaEntrega aleatoria entre mañana y +14 días
            var today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly minEntrega = today.AddDays(1);
            DateOnly maxEntrega = today.AddDays(14);
            int daysRange = (maxEntrega.DayNumber - minEntrega.DayNumber) + 1;
            int randomOffsetDays = Random.Shared.Next(daysRange);
            DateOnly entregaDateOnly = minEntrega.AddDays(randomOffsetDays);
            pedido.FechaEntrega = entregaDateOnly.ToDateTime(TimeOnly.MinValue);

            this.context.Pedidos.Add(pedido);
            await this.context.SaveChangesAsync();

            int idPedido = pedido.IdPedido;

            int nextDetalleId = await this.FindMaxIdDetallePedidoAsync() + 1;

            foreach (ProductoCarritoCantidad producto in productosValidos)
            {
                Producto? productoDb = await this.context.Productos
                    .FirstOrDefaultAsync(p => p.IdProducto == producto.IdProducto);

                if (productoDb == null)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }

                DetallePedido detallePedido = new DetallePedido
                {
                    IdDetallePedido = nextDetalleId,
                    IdPedido = idPedido,
                    IdProducto = producto.IdProducto,
                    Cantidad = producto.Cantidad,
                    PrecioUnitario = productoDb.Precio
                };

                nextDetalleId++;

                this.context.DetallePedidos.Add(detallePedido);

                int stockActual = productoDb.Stock ?? 0;
                productoDb.Stock = stockActual - producto.Cantidad;
                productoDb.Ventas += producto.Cantidad;

                this.context.Productos.Update(productoDb);
            }

            await this.context.SaveChangesAsync();
            await transaction.CommitAsync();

            return pedido.IdPedido;
        }

        private async Task<int> FindMaxIdPedidoAsync()
        {
            int maxId = this.context.Pedidos.Max(p => p.IdPedido);
            return maxId;
        }
        private async Task<int> FindMaxIdDetallePedidoAsync()
        {
            int maxId = this.context.DetallePedidos.Max(p => p.IdDetallePedido);
            return maxId;
        }
    }
}
