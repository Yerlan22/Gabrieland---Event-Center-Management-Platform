using System.ComponentModel.DataAnnotations;
namespace gabrieland.api.Models
{
    public class Salas
    {
        public int id_sala { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }
        
        [Required]
        public int Capacidad { get; set; }
        
        [Required]
        [Range(0, float.MaxValue)]
        public float Precio { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [RegularExpression("^[YN]$", ErrorMessage = "Activo solo acepta Y (Sí) o N (No)")]
        public string Activo { get; set; } // Consider using a bool instead if possible
    }
}