// Models/TiposUsuarios.cs
using System.ComponentModel.DataAnnotations;

namespace gabrieland.api.Models
{
    public class TiposUsuarios
    {
        public int ID_Tipos_usuario { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de usuario es requerido")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder 30 caracteres")]
        public string nombre { get; set; }
    }
}