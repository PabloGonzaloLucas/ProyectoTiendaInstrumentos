using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models
{
    [PrimaryKey(nameof(IdProducto), nameof(IdEspecificacion))]
    [Table("vw_EspecificacionesProducto")]
    public class VwEspecificacionesProducto
    {
        [Column("IdProducto")]
        public int IdProducto { get; set; }
        [Column("NombreProducto")]
        public string NombreProducto { get; set; }
        [Column("Especificacion")]
        public string Especificacion { get; set; }
        [Column("IdEspecificacion")]
        public int IdEspecificacion { get; set; }
        [Column("Valor")]
        public string Valor { get; set; }
        [Column("UnidadMedida")]
        public string UnidadMedida { get; set; }


    }
}
