using Microsoft.AspNetCore.Mvc;
using gabrieland.Data;
using gabrieland.api.Models;
using System.Threading.Tasks;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class TiposServiciosController : ControllerBase
    {
        private readonly DataBaseConnector _db;

        public TiposServiciosController(DataBaseConnector dataBaseConnector)
        {
            _db = dataBaseConnector;
        }

        // GET: /TiposServicios
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tiposServicios = await _db.GetTiposServicios();
            return Ok(tiposServicios);
        }

        // GET /TiposServicios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tipoServicio = await _db.GetTipoServicioById(id);
            if (tipoServicio == null)
            {
                return NotFound();
            }
            return Ok(tipoServicio);
        }

        // POST /TiposServicios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TiposServicios tipoServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Ensure incluido has a valid value
                tipoServicio.incluido = tipoServicio.incluido ?? "Y";

                var newId = await _db.CreateTipoServicio(tipoServicio);
                tipoServicio.ID_Tipo_Servicio = newId;

                return CreatedAtAction(nameof(Get), new { id = newId }, tipoServicio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al crear el tipo de servicio: " + ex.Message);
            }
        }

        // PUT /TiposServicios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TiposServicios tipoServicio)
        {
            if (id != tipoServicio.ID_Tipo_Servicio)
            {
                return BadRequest("ID del tipo de servicio no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _db.UpdateTipoServicio(tipoServicio);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar el tipo de servicio: " + ex.Message);
            }
        }

        // DELETE /TiposServicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // First check if exists
                var tipoServicio = await _db.GetTipoServicioById(id);
                if (tipoServicio == null)
                {
                    return NotFound("Tipo de servicio no encontrado");
                }

                var deleted = await _db.DeleteTipoServicio(id);
                if (!deleted)
                {
                    return StatusCode(500, "No se pudo eliminar el tipo de servicio");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al eliminar el tipo de servicio: " + ex.Message);
            }
        }
    }
}