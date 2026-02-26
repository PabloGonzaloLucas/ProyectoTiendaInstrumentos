using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("vw_HistorialPedidos")]
public partial class VwHistorialPedido
{
    [Key]
    [Column("IdPedido")]
    public int IdPedido { get; set; }
    [Column("Fecha")]

    public DateTime? Fecha { get; set; }
    [Column("Estado")]

    public string? Estado { get; set; }
    [Column("Cliente")]

    public string Cliente { get; set; } = null!;
    [Column("Email")]

    public string Email { get; set; } = null!;
    [Column("Producto")]

    public string Producto { get; set; } = null!;
    [Column("Marca")]

    public string Marca { get; set; } = null!;
    [Column("Cantidad")]

    public int Cantidad { get; set; }
    [Column("PrecioUnitario")]

    public decimal PrecioUnitario { get; set; }
    [Column("SubtotalLinea")]

    public decimal? SubtotalLinea { get; set; }
    [Column("TotalPedido")]

    public decimal TotalPedido { get; set; }
}
