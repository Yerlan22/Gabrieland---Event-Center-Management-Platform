
using System.ComponentModel.DataAnnotations;
namespace gabrieland.api.Models
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder 30 caracteres")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder 30 caracteres")]
        public string nombre { get; set; }

        [StringLength(30, ErrorMessage = "El apellido no puede exceder 30 caracteres")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(70, ErrorMessage = "El correo no puede exceder 70 caracteres")]
        public string correo { get; set; }

        [StringLength(20, ErrorMessage = "El número telefónico no puede exceder 20 caracteres")]
        public string num_telefonico { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(200, ErrorMessage = "El hash de contraseña no puede exceder 200 caracteres")]
        public string Hash_Contraseña { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime fecha_nacimiento { get; set; }

        [RegularExpression("^[YN]$", ErrorMessage = "Activo solo acepta Y (Sí) o N (No)")]
        public string Activo { get; set; } = "Y"; // Default to 'Y'

        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        public int TUO_ID_Tipo_Usuario { get; set; }
    }
}
