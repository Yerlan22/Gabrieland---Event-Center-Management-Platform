namespace gabrieland.Client.Models
{
    using System.Text.Json.Serialization;
    public class Salas
    {
        [JsonPropertyName("id_sala")]
        public int IdSala { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("capacidad")]
        public int Capacidad { get; set; }

        [JsonPropertyName("precio")]
        public float Precio { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("Activo")]
        public string Activo { get; set; } = string.Empty;
    }
}
