namespace gabrieland.Client.Models
{
    using System.Text.Json.Serialization;
    using System.ComponentModel.DataAnnotations;
    public class ReservaService
    {
        [JsonPropertyName("ID")]
        public int Id { get; set; }


        [JsonPropertyName("Cantidad")]
        public int Cantidad { get; set; }


        [JsonPropertyName("PrecioPago")]
        public float PrecioTotal { get; set; }


        [JsonPropertyName("ReservaId")]
        public int RES_Id_Reserva { get; set; }


        [JsonPropertyName("ServicioId")]
        public int SEA_Id_Servicio { get; set; }

        public string? Nombre { get; set; }
    }
}
