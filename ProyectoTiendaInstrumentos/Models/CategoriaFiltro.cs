using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models;

// Le indicas a EF Core cuáles son las dos columnas que forman la PK
[PrimaryKey(nameof(IdSubtipo), nameof(IdEspecificacion))]
[Table("CategoriaFiltros")]
public class CategoriaFiltro
{
    [Column("IdSubtipo")]
    public int IdSubtipo { get; set; }
    [Column("IdEspecificacion")]
    public int IdEspecificacion { get; set; }
}