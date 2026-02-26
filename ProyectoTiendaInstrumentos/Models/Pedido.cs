using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Pedidos")]
public partial class Pedido
{
    [Key]
    [Column("IdPedido")]
    public int IdPedido { get; set; }
    [Column("IdUsuario")]

    public int? IdUsuario { get; set; }
    [Column("Fecha")]

    public DateTime? Fecha { get; set; }
    [Column("Estado")]

    public string? Estado { get; set; }
    [Column("PrecioTotal")]

    public decimal PrecioTotal { get; set; }

}
