using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("SeguridadUsuarios")]
public partial class SeguridadUsuario
{
    [Key]
    [Column("IdUsuario")]
    public int IdUsuario { get; set; }
    [Column("Password")]

    public byte[] Password { get; set; } = null!;
    [Column("Salt")]

    public string Salt { get; set; } = null!;

}
