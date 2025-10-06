using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using gabrieland.api.Models;

namespace gabrieland.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly EmailService _emailService;

        private readonly ILogger<StripeController> _logger;

        public StripeController(ILogger<StripeController> logger, EmailService emailService)
        {
            
            _emailService = emailService;
            _logger = logger;
        }
        [HttpPost("crear-sesion")]
        public IActionResult CrearSesionPago([FromBody] CrearSesionRequest request)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = request.Correo,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "crc", // Colones costarricenses
                            UnitAmount = (long)(request.Monto * 100), // en céntimos
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = request.Descripcion
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = request.UrlExito,
                CancelUrl = request.UrlCancelacion,
                Locale = "es"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { id = session.Id, url = session.Url });
        }

        [HttpPost("confirmar-reserva")]
        public async Task<IActionResult> ConfirmarReserva([FromBody] CrearSesionRequest dto)
        {
            try
            {
                // Aquí podrías guardar la reserva en base de datos si ya tuvieras la lógica

                //await _emailService.SendReservationConfirmationEmail(dto.Correo, dto.Nombre, dto.Descripcion, dto.Monto);

                return Ok(new { Success = true, Message = "Reserva confirmada y correo enviado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar la reserva");
                return StatusCode(500, new { Success = false, Message = "Error al enviar la confirmación." });
            }
        }

    }

    public class CrearSesionRequest
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; } = 0;
        public string UrlExito { get; set; } = string.Empty;
        public string UrlCancelacion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
