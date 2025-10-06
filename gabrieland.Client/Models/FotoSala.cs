using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace gabrieland.Client.Models
{
    public class FotoSala
    {
        [JsonPropertyName("ID_foto")]
        public int IdFoto { get; set; }

        [JsonPropertyName("foto")]
        public string Foto { get; set; } = string.Empty;

        [JsonPropertyName("esPrincipal")]
        public string EsPrincipal { get; set; } = "Y"; // Default to 'Y'

        [JsonPropertyName("SAL_ID_Sala")]
        public int SalaId { get; set; }
    }
}