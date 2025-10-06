using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace gabrieland.Client.Models;
public enum MetodoPago
{
    TarjetaStripe = 21,
    Efectivo      = 22
}
/// View‑model para la página /pago
public class PagoInput : IValidatableObject
{
    [Required(ErrorMessage = "Seleccione un método de pago")]
    public MetodoPago? Metodo { get; set; }

    public string? NombreCompleto  { get; set; }
    public string? Identificacion  { get; set; }
    public string? correo          { get; set; }

    // Reglas que dependen del método de pago
    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        if (Metodo == MetodoPago.Efectivo)
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
                yield return new ValidationResult(
                    "El nombre es obligatorio para pago en efectivo",
                    new[] { nameof(NombreCompleto) });

            if (string.IsNullOrWhiteSpace(Identificacion))
                yield return new ValidationResult(
                    "La cédula es obligatoria",
                    new[] { nameof(Identificacion) });
            else if (!Identificacion.All(char.IsDigit))
                yield return new ValidationResult(
                    "La cédula debe contener solo números",
                    new[] { nameof(Identificacion) });
        }
    }
}
