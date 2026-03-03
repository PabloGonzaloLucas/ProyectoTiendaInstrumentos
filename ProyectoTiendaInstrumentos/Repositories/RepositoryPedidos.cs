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

            foreach (Pedido pedido in pedidos)
            {
                var idsProductos = from datos in this.context.DetallePedidos
                                           where datos.IdPedido == pedido.IdPedido
                                           select datos.IdProducto;

                //List<int> idsProductos = (await idsProductosConsulta.ToListAsync())
                //    .Where(x => x.HasValue)
                //    .Select(x => x.Value)
                //    .ToList();

                var imagenes = from datos in this.context.Productos
                               where idsProductos.Contains(datos.IdProducto)
                               select datos.Imagen;

                pedido.ImagenesProductos = await imagenes.ToListAsync();
            }

            return pedidos; // Ya está materializado, no re-ejecutar la consulta
        }

        public async Task<Pedido> GetPedidoByIdAsync(int idPedido)
        {
            var consulta = from datos in this.context.Pedidos
                           where datos.IdPedido == idPedido
                           select datos;

            Pedido pedido = await consulta.FirstOrDefaultAsync();

            var productosPedido = from datos in this.context.ProductosPedidos
                            where datos.IdPedido == idPedido
                            select datos;

            List<VwProductosPedido> productos = await productosPedido.ToListAsync();
            pedido.ProductosPedido = productos;

            return pedido; // Ya está materializado, no re-ejecutar la consulta
        }
    
    }
}
