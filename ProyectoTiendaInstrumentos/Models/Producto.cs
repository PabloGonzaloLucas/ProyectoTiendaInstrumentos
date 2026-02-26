using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Productos")]
public class Producto
{
    [Key]
    [Column("IdProducto")]
    public int IdProducto { get; set; }
    [Column("IdSubtipo")]

    public int? IdSubtipo { get; set; }
    [Column("IdMarca")]

    public int? IdMarca { get; set; }
    [Column("Modelo")]

    public string Modelo { get; set; } = null!;
    [Column("Descripcion")]

    public string? Descripcion { get; set; }
    [Column("Precio")]

    public decimal Precio { get; set; }
    [Column("Stock")]

    public int? Stock { get; set; }
    [Column("Puntuacion")]

    public int Puntuacion { get; set; }
    [Column("Ventas")]

    public int Ventas { get; set; }
    [Column("Imagen")]

    public string? Imagen { get; set; }
    [Column("FechaCreacion")]

    public DateTime? FechaCreacion { get; set; }



}
