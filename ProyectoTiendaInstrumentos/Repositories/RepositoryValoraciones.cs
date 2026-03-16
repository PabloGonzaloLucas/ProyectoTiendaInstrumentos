using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryValoraciones
    {
        private ProyectoTiendaInstrumentosContext context;
        public RepositoryValoraciones(ProyectoTiendaInstrumentosContext context)
        {
            this.context = context;
        }

        public async Task<List<VwValoracionesProducto>> GetValoracionesByProductoAsync(int idProducto)
        {
            var consulta = from datos in this.context.ValoracionesProductos
                           where datos.IdProducto == idProducto
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task<bool> UsuarioHaValoradoProductoAsync(int idUsuario, int idProducto)
        {
            return await this.context.Valoraciones
                .AnyAsync(v => v.IdUsuario == idUsuario && v.IdProducto == idProducto);
        }

        public async Task<bool> UsuarioHaCompradoProductoAsync(int idUsuario, int idProducto)
        {
            var haComprado = await this.context.DetallePedidos
                .Where(detalle => detalle.IdProducto == idProducto)
                .AnyAsync(detalle => this.context.Pedidos
                    .Any(pedido => pedido.IdPedido == detalle.IdPedido
                                && pedido.IdUsuario == idUsuario
                                && pedido.Estado != null));

            return haComprado;
        }

        public async Task<int> AddValoracionAsync(int idUsuario, int idProducto, int puntuacion, string comentario)
        {
            bool yaValorado = await UsuarioHaValoradoProductoAsync(idUsuario, idProducto);

            if (yaValorado)
            {
                return 0;
            }

            bool haComprado = await UsuarioHaCompradoProductoAsync(idUsuario, idProducto);

            if (!haComprado)
            {
                return -1;
            }

            int maxId = await this.context.Valoraciones.AnyAsync()
                ? await this.context.Valoraciones.MaxAsync(v => v.Id) + 1
                : 1;

            Valoracion nuevaValoracion = new Valoracion
            {
                Id = maxId,
                IdUsuario = idUsuario,
                IdProducto = idProducto,
                Puntuacion = puntuacion,
                Comentario = comentario,
                FechaCreacion = DateTime.Now
            };

            await this.context.Valoraciones.AddAsync(nuevaValoracion);
            await this.context.SaveChangesAsync();

            await ActualizarPuntuacionProductoAsync(idProducto);
            await ActualizarPuntuacionMarcaPorProductoAsync(idProducto);

            return nuevaValoracion.Id;
        }

        private async Task ActualizarPuntuacionProductoAsync(int idProducto)
        {
            var valoraciones = await this.context.Valoraciones
                .Where(v => v.IdProducto == idProducto)
                .ToListAsync();

            if (valoraciones.Count > 0)
            {
                int promedio = (int)Math.Round(valoraciones.Average(v => v.Puntuacion));

                var producto = await this.context.Productos
                    .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

                if (producto != null)
                {
                    producto.Puntuacion = promedio;
                    await this.context.SaveChangesAsync();
                }
            }
        }

        private async Task ActualizarPuntuacionMarcaPorProductoAsync(int idProducto)
        {
            // Obtenemos la marca del producto valorado
            var producto = await this.context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            if (producto?.IdMarca == null)
            {
                return;
            }

            int idMarca = producto.IdMarca.Value;

            // Media de todas las valoraciones de productos de la marca
            // (equivale a media ponderada de todas las valoraciones registradas para la marca)
            var puntuacionesMarca = await (
                    from v in this.context.Valoraciones
                    join p in this.context.Productos on v.IdProducto equals p.IdProducto
                    where p.IdMarca == idMarca
                    select v.Puntuacion
                )
                .ToListAsync();

            if (puntuacionesMarca.Count == 0)
            {
                return;
            }

            int promedioMarca = (int)Math.Round(puntuacionesMarca.Average());

            var marca = await this.context.Marcas
                .FirstOrDefaultAsync(m => m.IdMarca == idMarca);

            if (marca == null)
            {
                return;
            }

            marca.Puntuacion = promedioMarca;
            await this.context.SaveChangesAsync();
        }
    }
}
