using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Usuarios")]
public partial class Usuario
{
    [Key]
    [Column("IdUsuario")]
    public int IdUsuario { get; set; }
    [Column("RolId")]

    public int? RolId { get; set; }
    [Column("Nombre")]

    public string Nombre { get; set; } = null!;
    [Column("Email")]

    public string Email { get; set; } = null!;
    [Column("PasswordFake")]

    public string PasswordFake { get; set; } = null!;
    [Column("Direccion")]

    public string Direccion { get; set; } = null!;
    [Column("Telefono")]

    public string? Telefono { get; set; }
    [Column("Imagen")]

    public string Imagen { get; set; } = null!;
    [Column("FechaRegistro")]

    public DateTime? FechaRegistro { get; set; }

    [NotMapped]
    public int? NumPedidos { get; set; }

}
