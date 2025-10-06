using gabrieland.api.Data;
using gabrieland.Data;
using gabrieland.api.Models;
using gabrieland.api.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ReservasData _db;
        private readonly DataBaseConnector _dbUsuarios;
        private readonly EmailService _emailService;
        public ReservasController(ReservasData DBReservas, DataBaseConnector
        
         dbUsuarios, EmailService emailService)
        {
            _db = DBReservas;
            _dbUsuarios = dbUsuarios;
            _emailService = emailService;
        }

        // GET: /<ReservasController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> Get([FromQuery] string? estado)
        {
            try
            {
                var reservas = await _db.GetReservas(estado);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET /<ReservasController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> Get(int id)
        {
            try
            {
                var reserva = await _db.GetReservaById(id);
                if (reserva == null)
                {
                    return NotFound();
                }
                return Ok(reserva);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET /Usuarios/{id}/Reservas?estado=Pendiente
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetReservasDeUsuario(
                int usuarioId)
        {
            var lista = await _db.GetReservasByUsuario(usuarioId);
            return Ok(lista);
        }

        // POST api/<ReservasController>
        [HttpPost]
        public async Task<ActionResult<Reserva>> Post([FromBody] Reserva reserva)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var id = await _db.CreateReserva(reserva);
                Console.WriteLine(id);
                reserva.IdReserva = id;
                Console.WriteLine(reserva.IdReserva);
                return CreatedAtAction(nameof(Get), new {id = reserva.IdReserva}, reserva);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/<ReservasController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Reserva reserva)
        {
            try
            {
                if (id != reserva.IdReserva)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var actual = await _db.GetReservaById(id);
                if (actual is null)
                    return NotFound(); 

                bool enviarCorreo =
                    actual.Estado == EstadoReserva.Pendiente &&
                    reserva.Estado  == EstadoReserva.Confirmada;

                var updated = await _db.UpdateReserva(reserva);
                if (!updated)
                    return NotFound();

                if (enviarCorreo)
                {
                    var usuario  = await _dbUsuarios.GetUsuarioById(reserva.UsuarioId);
                   if (usuario is not null)
                    {
                        await _emailService.SendReservationConfirmationEmail(
                                toEmail: usuario.correo,
                                userFullName: $"{usuario.nombre} {usuario.apellido}",
                                reservaId: id);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/<ReservasController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _db.LogicalDeleteReserva(id);
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}