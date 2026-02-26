using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Valoraciones")]
public partial class Valoracion
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }
    [Column("IdProducto")]

    public int? IdProducto { get; set; }
    [Column("IdUsuario")]

    public int? IdUsuario { get; set; }
    [Column("Puntuacion")]

    public int Puntuacion { get; set; }
    [Column("Comentario")]

    public string? Comentario { get; set; }
    [Column("FechaCreacion")]

    public DateTime? FechaCreacion { get; set; }

}
