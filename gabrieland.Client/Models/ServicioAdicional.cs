using System.ComponentModel.DataAnnotations;

namespace gabrieland.Client.Models
{
    public class ServicioAdicional
    {
        public int ID_Servicios_Adicionales { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string nombre { get; set; } = string.Empty;

        [StringLength(512, ErrorMessage = "La descripción no puede exceder 512 caracteres")]
        public string descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El costo es requerido")]
        [Range(0, float.MaxValue, ErrorMessage = "El costo no puede ser negativo")]
        public float costo { get; set; }

        [RegularExpression("^[YN]$", ErrorMessage = "Activo solo acepta Y (Sí) o N (No)")]
        public string Activo { get; set; } = "Y"; // Default to 'Y'

        [Required(ErrorMessage = "El tipo de servicio es requerido")]
        public int TSO_ID_Tipo_Servicio { get; set; }
    }
}