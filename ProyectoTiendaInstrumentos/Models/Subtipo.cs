using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Subtipos")]
public partial class Subtipo
{
    [Key]
    [Column("IdSubtipo")]
    public int IdSubtipo { get; set; }
    [Column("IdTipo")]

    public int? IdTipo { get; set; }
    [Column("Nombre")]

    public string Nombre { get; set; } = null!;


}
