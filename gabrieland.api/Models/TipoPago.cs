using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gabrieland.api.Models
{
    public class TipoPago
    {
        [Key]
        [Column("ID_Tipo_pago")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(30)]
        public string Nombre { get; set; }

        [Column("descripcion")]
        [StringLength(512)]
        public string? Descripcion { get; set; }
    }
}
