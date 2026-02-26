using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

[Table("ValoresEspecificaciones")]
public partial class ValoresEspecificacion
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }
    [Column("IdProducto")]

    public int? IdProducto { get; set; }
    [Column("IdEspecificacion")]

    public int? IdEspecificacion { get; set; }
    [Column("Valor")]

    public string Valor { get; set; } = null!;

}
