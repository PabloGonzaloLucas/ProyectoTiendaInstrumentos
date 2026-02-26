using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Especificaciones")]
public partial class Especificacion
{
    [Key]
    [Column("IdEspecificacion")]
    public int IdEspecificacion { get; set; }
    [Column("Nombre")]

    public string Nombre { get; set; } = null!;
    [Column("UnidadMedida")]

    public string? UnidadMedida { get; set; }

}
