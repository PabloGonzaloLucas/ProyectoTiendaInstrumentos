using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("ProductoImagenes")]
public partial class ProductoImagen
{
    [Key]
    [Column("IdProductoImagen")]
    public int IdProductoImagen { get; set; }
    [Column("IdProducto")]

    public int? IdProducto { get; set; }
    [Column("Imagen")]

    public string Imagen { get; set; } = null!;

}
