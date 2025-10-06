using System.ComponentModel.DataAnnotations;

namespace gabrieland.Client.Models
{
    public class TiposServicios
    {
        public int ID_Tipo_Servicio { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder 30 caracteres")]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo incluido es requerido")]
        [RegularExpression("^[YN]$", ErrorMessage = "Incluido solo acepta Y (Sí) o N (No)")]
        public string incluido { get; set; } = "Y"; // Default to 'Y'
    }
}