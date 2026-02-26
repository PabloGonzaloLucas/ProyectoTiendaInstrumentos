using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("DetallePedidos")]
public partial class DetallePedido
{
    [Key]
    [Column("IdDetallePedido")]
    public int IdDetallePedido { get; set; }
    [Column("IdPedido")]

    public int? IdPedido { get; set; }
    [Column("IdProducto")]

    public int? IdProducto { get; set; }
    [Column("Cantidad")]

    public int Cantidad { get; set; }
    [Column("PrecioUnitario")]

    public decimal PrecioUnitario { get; set; }


}
