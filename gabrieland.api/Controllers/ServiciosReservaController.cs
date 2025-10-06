using gabrieland.api.Data;
using gabrieland.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ServiciosReservaController : ControllerBase
    {
        private readonly ServicioReservaData _db;

        public ServiciosReservaController(ServicioReservaData db)
        {
            _db = db;
        }

        // GET: api/ServiciosReserva
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicioReserva>>> GetAll()
        {
            try
            {
                var servicios = await _db.GetAll();
                return Ok(servicios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/ServiciosReserva/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioReserva>> GetById(int id)
        {
            try
            {
                var servicio = await _db.GetById(id);
                if (servicio == null)
                {
                    return NotFound();
                }
                return Ok(servicio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/ServiciosReserva/reserva/5
        [HttpGet("reserva/{reservaId}")]
        public async Task<ActionResult<IEnumerable<ServicioReserva>>> GetByReservaId(int reservaId)
        {
            try
            {
                var servicios = await _db.GetByReservaId(reservaId);
                return Ok(servicios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/ServiciosReserva
        [HttpPost]
        public async Task<ActionResult<ServicioReserva>> Create([FromBody] ServicioReserva servicioReserva)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var id = await _db.Create(servicioReserva);
                servicioReserva.ID = id;

                return CreatedAtAction(nameof(GetById), new { id }, servicioReserva);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/ServiciosReserva/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServicioReserva servicioReserva)
        {
            try
            {
                if (id != servicioReserva.ID)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _db.Update(servicioReserva);
                if (!updated)
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

        // DELETE: api/ServiciosReserva/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _db.Delete(id);
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