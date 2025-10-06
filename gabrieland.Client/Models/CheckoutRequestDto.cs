namespace gabrieland.Client.Models;

/// <summary>
/// Respuesta del endpoint POST /reservas/checkout.
/// Contiene los datos necesarios para redirigir a Stripe.
/// </summary>
public class CheckoutRequestDto
{
    public string SessionId { get; set; } = default!;
    public string url { get; set; } = default!;
}
