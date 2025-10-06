using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gabrieland.api.Models
{
    public class ServicioReserva
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("cantidad")]
        [Range(0,int.MaxValue,ErrorMessage = "No se permiten numeros negativos")]
        public int Cantidad { get; set; }

        [Column("Precio_pago")]
        public decimal PrecioPago { get; set; }

        [Column("RES_ID_Reserva")]
        public int ReservaId { get; set; }

        [Column("SAE_ID_servicio")]
        public int ServicioId { get; set; }
    }
}
