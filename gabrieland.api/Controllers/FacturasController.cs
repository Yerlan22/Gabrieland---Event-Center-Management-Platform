using gabrieland.api.Data;
using gabrieland.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly FacturaData _db;

        public FacturasController(FacturaData db)
        {
            _db = db;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetAll()
        {
            try
            {
                var facturas = await _db.GetAll();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetById(int id)
        {
            try
            {
                var factura = await _db.GetById(id);
                if (factura == null)
                {
                    return NotFound();
                }
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Facturas/reserva/5
        [HttpGet("reserva/{reservaId}")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetByReservaId(int reservaId)
        {
            try
            {
                var facturas = await _db.GetByReservaId(reservaId);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Facturas
        [HttpPost]
        public async Task<ActionResult<Factura>> Create([FromBody] CreateFacturaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var id = await _db.CreateFactura(request.ReservaId, request.TipoPagoId);
                var factura = await _db.GetById(id);

                return CreatedAtAction(nameof(GetById), new { id }, factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public record CreateFacturaRequest(int ReservaId, int TipoPagoId);
    }
}