using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gabrieland.api.Models
{
    // Factura.cs
    public class Factura
    {
        [Key]
        [Column("ID_Factura")]
        public int Id { get; set; }

        [Required]
        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [Required]
        [Column("monto_sala")]
        [Range(0, double.MaxValue)]
        public decimal MontoSala { get; set; }

        [Required]
        [Column("monto_servicios_adicionales")]
        [Range(0, double.MaxValue)]
        public decimal MontoServiciosAdicionales { get; set; }

        [Required]
        [Column("RES_ID_Reserva")]
        public int ReservaId { get; set; }

        [Required]
        [Column("TPO_ID_Tipo_pago")]
        public int TipoPagoId { get; set; }

        [NotMapped]
        public decimal Total => MontoSala + MontoServiciosAdicionales;
    }

}
