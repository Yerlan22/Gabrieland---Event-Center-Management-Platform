using gabrieland.api.Data;
using gabrieland.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class TiposPagoController : ControllerBase
    {
        private readonly TiposPagoData _db;

        public TiposPagoController(TiposPagoData db)
        {
            _db = db;
        }

        // GET: api/TiposPago
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoPago>>> GetAll()
        {
            try
            {
                var tipos = await _db.GetAll();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/TiposPago/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoPago>> GetById(int id)
        {
            try
            {
                var tipo = await _db.GetById(id);
                if (tipo == null)
                {
                    return NotFound();
                }
                return Ok(tipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/TiposPago
        [HttpPost]
        public async Task<ActionResult<TipoPago>> Create([FromBody] TipoPago tipoPago)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var id = await _db.Create(tipoPago);
                tipoPago.Id = id;

                return CreatedAtAction(nameof(GetById), new { id }, tipoPago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/TiposPago/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoPago tipoPago)
        {
            try
            {
                if (id != tipoPago.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _db.Update(tipoPago);
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

        // DELETE: api/TiposPago/5
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