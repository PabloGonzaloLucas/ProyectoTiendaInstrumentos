using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Data
{
    public class ProyectoTiendaInstrumentosContext : DbContext
    {
        public ProyectoTiendaInstrumentosContext(DbContextOptions<ProyectoTiendaInstrumentosContext> opt) : base(opt)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Especificacion> Especificaciones { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tipo> Tipos { get; set; }
        public DbSet<Subtipo> Subtipos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Valoracion> Valoraciones { get; set; }
        public DbSet<ValoresEspecificacion> ValoresEspecifiaciones { get; set; }
        public DbSet<ProductoImagen> ProductosImagenes { get; set; }
        public DbSet<SeguridadUsuario> SeguridadUsuarios { get; set; }
        public DbSet<VwCatalogoProducto> CatalogoProductos { get; set; }
        public DbSet<VwFichaTecnicaProducto> FichaTecnicaProductos { get; set; }
        public DbSet<VwHistorialPedido> HistorialPedidos { get; set; }
        public DbSet<VwEspecificacionesProducto> EspecificacionesProducto { get; set; }
    }
}
