using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Familias")]
public partial class Familia
{
    [Key]
    [Column("IdFamilia")]
    public int IdFamilia { get; set; }
    [Column("Nombre")]

    public string Nombre { get; set; } = null!;
    [Column("ImagenBanner")]

    public string? ImagenBanner { get; set; }

}
