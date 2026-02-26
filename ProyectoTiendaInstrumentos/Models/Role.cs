using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Roles")]
public partial class Role
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }
    [Column("NombreRol")]

    public string NombreRol { get; set; } = null!;

}
