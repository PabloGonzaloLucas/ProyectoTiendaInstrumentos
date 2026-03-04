using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories.Interfaces
{
    public interface IRepositoryCarrito
    {
        Task AddProductoToCartAsync(int idProducto, int idUsuario, int cantidad);
        Task<List<VwCatalogoProducto>> GetProductosCarritoByUsuarioAsync(int idUsuario);
        Task RemoveProductoFromCartAsync(int idProducto, int idUsuario);
        Task ClearCartAsync(int idUsuario);
        Task<int> GetNumProductosCarritoAsync(int idUsuario);
        Task ActualizarCantidadAsync(int idProducto, int cantidad, int idUsuario);
        Task<int> ComprarProductosAsync(List<ProductoCarritoCantidad> productos, int idUsuario);
    }
}
