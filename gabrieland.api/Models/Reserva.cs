using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gabrieland.api.Models
{
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

    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        [Column("ID_Reserva")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        [Column("duracion")]
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que 0")]
        public int Duracion { get; set; } // in hours

        [Required(ErrorMessage = "El estado es requerido")]
        [Column("estado")]
        public EstadoReserva Estado { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [Column("fecha")]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El ID de sala es requerido")]
        [Column("SAL_ID_Sala")]
        [ForeignKey("Sala")]
        public int SalaId { get; set; }

        [Required(ErrorMessage = "El ID de usuario es requerido")]
        [Column("USU_ID_Usuario")]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        // Additional calculated properties
        [NotMapped]
        public DateTime FechaFin => Fecha.AddHours(Duracion);
        [NotMapped]
        public int Hora_Inicio => Fecha.Hour;
        [NotMapped]
        public int Hora_Final => Fecha.AddHours(Duracion).Hour;
        [NotMapped]
        public string EstadoDisplayName
        {
            get
            {
                var fieldInfo = Estado.GetType().GetField(Estado.ToString());
                var descriptionAttributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
                return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Name : Estado.ToString();
            }
        }
    }

}
