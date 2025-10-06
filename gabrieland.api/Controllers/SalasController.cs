using Microsoft.AspNetCore.Mvc;
using gabrieland.Data;
using gabrieland.api.Models;
using System.Threading.Tasks;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class SalasController : ControllerBase
    {
        private readonly DataBaseConnector _db;

        public SalasController(DataBaseConnector dataBaseConnector)
        {
            _db = dataBaseConnector;
        }

        // GET /Salas
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool incluirInactivos = false)
        {
            var salas = await _db.GetSalas(incluirInactivos);
            return Ok(salas);
        }

        // GET /Salas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery] bool incluirInactivos = false)
        {
            var sala = await _db.GetSalaById(id, incluirInactivos);
            if (sala == null)
            {
                return NotFound();
            }
            return Ok(sala);
        }

        // POST /Salas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Salas sala)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Set default value for Activo if not provided
                sala.Activo = sala.Activo ?? "Y";

                var newId = await _db.CreateSala(sala);
                sala.id_sala = newId;

                return CreatedAtAction(nameof(Get), new { id = newId }, sala);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al crear la sala: " + ex.Message);
            }
        }

        // PUT /Salas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Salas sala)
        {
            if (id != sala.id_sala)
            {
                return BadRequest("ID de la sala no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _db.UpdateSala(sala);
                if (!updated)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar la sala:" + ex.Message);
            }
        }

        // DELETE /Salas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // First check if sala exists
                var sala = await _db.GetSalaById(id);
                if (sala == null)
                {
                    return NotFound("Sala no encontrada");
                }

                // Check if already inactive
                if (sala.Activo == "N")
                {
                    return BadRequest("La sala ya está inactiva");
                }

                var deactivated = await _db.LogicalDeleteSala(id);
                if (!deactivated)
                {
                    return StatusCode(500, "No se pudo desactivar la sala");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al desactivar la sala: " + ex.Message);
            }
        }

        // PATCH /Salas/{id}/reactivar
        [HttpPatch("{id}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            try
            {
                // First check if sala exists (including inactive ones)
                var sala = await _db.GetSalaById(id);
                if (sala == null)
                {
                    return NotFound("Sala no encontrada");
                }

                // Check if already active
                if (sala.Activo == "Y")
                {
                    return BadRequest("La sala ya está activa");
                }

                // Reactivate the sala
                var reactivated = await _db.ReactivateSala(id);
                if (!reactivated)
                {
                    return StatusCode(500, "No se pudo reactivar la sala");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al reactivar la sala: " + ex.Message +"\n\n" + ex.StackTrace);
            }
        }
    }
}