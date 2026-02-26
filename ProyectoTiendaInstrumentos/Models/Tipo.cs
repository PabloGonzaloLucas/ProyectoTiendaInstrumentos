using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Tipos")]
public partial class Tipo
{
    [Key]
    [Column("IdTipo")]
    public int IdTipo { get; set; }
    [Column("IdFamilia")]

    public int? IdFamilia { get; set; }
    [Column("Nombre")]

    public string Nombre { get; set; } = null!;


}
