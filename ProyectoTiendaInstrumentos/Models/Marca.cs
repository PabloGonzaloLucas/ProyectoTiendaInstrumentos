using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("Marcas")]
public partial class Marca
{
    [Key]
    [Column("IdMarca")]
    public int IdMarca { get; set; }
    [Column("Nombre")]

    public string Nombre { get; set; } = null!;
    [Column("Logo")]

    public string Logo { get; set; } = null!;
    [Column("Puntuacion")]

    public int Puntuacion { get; set; }
}
