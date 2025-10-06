using System.ComponentModel.DataAnnotations;
namespace gabrieland.api.Models
{
    public class LoginDto
    {
         [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contraseña { get; set; }
    }
}