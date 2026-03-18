using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("vw_CatalogoProductos")]
public partial class VwCatalogoProducto
{
    [Key]
    [Column("IdProducto")]
    public int IdProducto { get; set; }
    [Column("Modelo")]

    public string Modelo { get; set; } = null!;
    [Column("Marca")]

    public string Marca { get; set; } = null!;
    [Column("Precio")]

    public decimal Precio { get; set; }
    [Column("Stock")]

    public int? Stock { get; set; }
    [Column("Ventas")]

    public int? Ventas { get; set; }
    [Column("Estrellas")]

    public int Estrellas { get; set; }
    [Column("Subtipo")]

    public string Subtipo { get; set; } = null!;
    [Column("IdSubtipo")]

    public int IdSubtipo { get; set; }
    [Column("Tipo")]

    public string Tipo { get; set; } = null!;
    [Column("Familia")]

    public string Familia { get; set; } = null!;
    [Column("ImagenPrincipal")]

    public string? ImagenPrincipal { get; set; }

    [NotMapped]
    public List<VwEspecificacionesProducto>? Especificaciones { get; set; }
    [NotMapped]
    public int? Cantidad { get; set; }
}
