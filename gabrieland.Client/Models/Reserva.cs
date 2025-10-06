namespace gabrieland.Client.Models
{   
    using System.Text.Json.Serialization;
    using System.ComponentModel.DataAnnotations;
    public enum EstadoReserva
    {
        [Display(Name = "Pendiente")]
        Pendiente,

        [Display(Name = "Confirmada")]
        Confirmada,

        [Display(Name = "En Curso")]
        EnCurso,

        [Display(Name = "Completada")]
        Completada,

        [Display(Name = "Cancelada")]
        Cancelada
    }
    public class Reserva
    {
        [JsonPropertyName("IdReserva")]
        public int Id_Reserva { get; set; }
        
        public int Hora_Inicio { get; set; }
        public int Hora_Final { get; set; }

        [JsonPropertyName("duracion")]
        public int Duracion { get; set; }

        [JsonPropertyName("estado")]
        public EstadoReserva Estado_Reserva { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("salaId")]
        public int SAL_Id_Sala { get; set; }

        [JsonPropertyName("usuarioId")]
        public int USU_Id_Usuario { get; set; }
    }
}
