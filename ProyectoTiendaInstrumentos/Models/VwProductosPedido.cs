using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models
{
    [Table("vw_ProductosPedidos")]
    [PrimaryKey(nameof(IdProducto), nameof(IdPedido))]

    public class VwProductosPedido
    {
        [Column("IdProducto")]
        public int IdProducto{ get; set; }

        [Column("IdPedido")]
        public int IdPedido { get; set; }
        [Column("Modelo")]

        public string Modelo { get; set; } = null!;

        [Column("Marca")]

        public string Marca { get; set; } = null!;
        [Column("Imagen")]

        public string Imagen { get; set; } = null!;
        [Column("Cantidad")]

        public int Cantidad { get; set; }
        [Column("PrecioUnitario")]

        public decimal PrecioUnitario { get; set; }
        [Column("SubtotalLinea")]

        public decimal? SubtotalLinea { get; set; }
  
    }
}
