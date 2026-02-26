using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("vw_FichaTecnicaProductos")]
public partial class VwFichaTecnicaProducto
{
    [Key]
    [Column("IdProducto")]
    public int IdProducto { get; set; }
    [Column("Producto")]

    public string Producto { get; set; } = null!;
    [Column("Caracteristica")]

    public string Caracteristica { get; set; } = null!;
    [Column("Valor")]

    public string Valor { get; set; } = null!;
    [Column("Unidad")]

    public string Unidad { get; set; } = null!;
}
