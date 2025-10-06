using System.ComponentModel.DataAnnotations;

namespace gabrieland.Client.Models
{
    using System.Text.Json.Serialization;
    public class Usuario
    {
        public int ID_Usuario { get; set; } // ID artificial de Usuario

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder 30 caracteres")]
        [RegularExpression("^[a-zA-ZÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras")]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(30, ErrorMessage = "El apellido no puede exceder 30 caracteres")]
        [RegularExpression("^[a-zA-ZÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El apellido solo puede contener letras")]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(30, ErrorMessage = "El nombre de usuario no puede exceder 30 caracteres")]
        public string UserName { get; set; } = string.Empty; // Nombre de Usuario

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(70, ErrorMessage = "El correo no puede exceder 70 caracteres")]
        public string correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número telefónico es requerido")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El número telefónico debe contener exactamente 8 dígitos")]
        public string num_telefonico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(200, ErrorMessage = "El hash de contraseña no puede exceder 200 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{8,}$",
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula y un símbolo.")]
        public string contrasena { get; set; } = string.Empty;

        [JsonPropertyName("Hash_Contraseña")]
        public string hash_password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime fecha_nacimiento { get; set; } = DateTime.Now;

        [RegularExpression("^[YN]$", ErrorMessage = "Activo solo acepta Y (Sí) o N (No)")]
        public string activo { get; set; } = "Y"; // Default to 'Yes'

        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        public int TUO_ID_Tipo_Usuario { get; set; }
    }
}
