namespace gabrieland.Client.Models;

/// <summary>
/// Cuerpo JSON que envía el cliente cuando elige pagar en efectivo.
/// </summary>
public class PagoReserva
{
    public int ReservaId { get; set; }
    public decimal Monto { get; set; }
    public MetodoPago Metodo { get; set; }
    public string? Nombre { get; set; }
    public string? Identificacion { get; set; }
}
