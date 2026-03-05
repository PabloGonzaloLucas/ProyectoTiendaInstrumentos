using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models
{
    [Table("vw_ValoracionesProducto")]
    [Keyless]
    public class VwValoracionesProducto
    {
        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("Puntuacion")]

        public int Puntuacion { get; set; }

        [Column("Comentario")]

        public string Comentario { get; set; }
        [Column("ImagenUsuario")]

        public string ImagenUsuario { get; set; } = null!;
        [Column("NombreUsuario")]

        public string NombreUsuario { get; set; }
    
        [Column("FechaCreacion")]

        public DateTime FechaCreacion { get; set; }


    }
}
