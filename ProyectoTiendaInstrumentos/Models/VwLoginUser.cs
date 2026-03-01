using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoTiendaInstrumentos.Models
{
    [Table("vw_LoginUser")]
    public class VwLoginUser
    {
        [Key]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }
        [Column("Email")]
        public string Email { get; set; } = null!;
        [Column("Password")]

        public byte[] Password { get; set; } = null!;
        [Column("Salt")]
        public string Salt { get; set; } = null!;

    }
}
