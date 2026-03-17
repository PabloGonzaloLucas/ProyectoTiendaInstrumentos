using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryPedidos
    {
        private ProyectoTiendaInstrumentosContext context;

        public RepositoryPedidos(ProyectoTiendaInstrumentosContext context)
        {
            this.context = context;
        }

        public async Task<List<Pedido>> GetPedidosByUsuarioAsync(int idUsuario)
        {
            var consulta = from datos in this.context.Pedidos
                           where datos.IdUsuario == idUsuario
                           select datos;

            // Materializar la lista ANTES del foreach para cerrar el DataReader
            List<Pedido> pedidos = await consulta.ToListAsync();

            // Actualizar estado a 'Entregado' si la fecha de entrega ya ha pasado
            DateTime now = DateTime.Now;
            bool anyChanged = false;
            foreach (Pedido pedido in pedidos)
            {
                if (pedido.FechaEntrega.HasValue
                    && pedido.FechaEntrega.Value < now
                    && !string.Equals(pedido.Estado, "Entregado", StringComparison.OrdinalIgnoreCase))
                {
                    pedido.Estado = "Entregado";
                    anyChanged = true;
                }

                var idsProductos = from datos in this.context.DetallePedidos
                                   where datos.IdPedido == pedido.IdPedido
                                   select datos.IdProducto;

                var imagenes = from datos in this.context.Productos
                               where idsProductos.Contains(datos.IdProducto)
                               select datos.Imagen;

                pedido.ImagenesProductos = await imagenes.ToListAsync();
            }

            if (anyChanged)
            {
                await this.context.SaveChangesAsync();
            }

            return pedidos;
        }

        public async Task<Pedido> GetPedidoByIdAsync(int idPedido)
        {
            var consulta = from datos in this.context.Pedidos
                           where datos.IdPedido == idPedido
                           select datos;

            Pedido pedido = await consulta.FirstOrDefaultAsync();

            if (pedido == null)
            {
                return null;
            }

            // Actualizar estado a 'Entregado' si la fecha de entrega ya ha pasado
            if (pedido.FechaEntrega.HasValue
                && pedido.FechaEntrega.Value < DateTime.Now
                && !string.Equals(pedido.Estado, "Entregado", StringComparison.OrdinalIgnoreCase))
            {
                pedido.Estado = "Entregado";
                await this.context.SaveChangesAsync();
            }

            var productosPedido = from datos in this.context.ProductosPedidos
                                  where datos.IdPedido == idPedido
                                  select datos;

            List<VwProductosPedido> productos = await productosPedido.ToListAsync();
            pedido.ProductosPedido = productos;

            return pedido;
        }

    }
}
