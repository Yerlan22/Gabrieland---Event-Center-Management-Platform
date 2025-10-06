using Microsoft.AspNetCore.Mvc;
using gabrieland.Data;
using gabrieland.api.Models;
using System.Threading.Tasks;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ServiciosAdicionalesController : ControllerBase
    {
        private readonly DataBaseConnector _db;

        public ServiciosAdicionalesController(DataBaseConnector dataBaseConnector)
        {
            _db = dataBaseConnector;
        }

        // GET: /ServiciosAdicionales
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool incluirInactivos = false)
        {
            var servicios = await _db.GetServiciosAdicionales(incluirInactivos);
            return Ok(servicios);
        }

        // GET /ServiciosAdicionales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery] bool incluirInactivos = false)
        {
            var servicio = await _db.GetServicioAdicionalById(id, incluirInactivos);
            if (servicio == null)
            {
                return NotFound();
            }
            return Ok(servicio);
        }

        // POST /ServiciosAdicionales
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ServicioAdicional servicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                servicio.Activo = "Y"; // Ensure new services are active by default
                var newId = await _db.CreateServicioAdicional(servicio);
                servicio.ID_Servicios_Adicionales = newId;

                return CreatedAtAction(nameof(Get), new { id = newId }, servicio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al crear el servicio adicional: " + ex.Message);
            }
        }

        // PUT /ServiciosAdicionales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ServicioAdicional servicio)
        {
            if (id != servicio.ID_Servicios_Adicionales)
            {
                return BadRequest("ID del servicio no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _db.UpdateServicioAdicional(servicio);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar el servicio adicional" + ex.Message);
            }
        }

        // DELETE /ServiciosAdicionales/5 (logical delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var servicio = await _db.GetServicioAdicionalById(id, true);
                if (servicio == null)
                {
                    return NotFound("Servicio no encontrado");
                }

                if (servicio.Activo == "N")
                {
                    return BadRequest("El servicio ya está inactivo");
                }

                var deleted = await _db.LogicalDeleteServicioAdicional(id);
                if (!deleted)
                {
                    return StatusCode(500, "No se pudo desactivar el servicio");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al desactivar el servicio" + ex.Message);
            }
        }

        // PATCH /ServiciosAdicionales/5/reactivar
        [HttpPatch("{id}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            try
            {
                var servicio = await _db.GetServicioAdicionalById(id, true);
                if (servicio == null)
                {
                    return NotFound("Servicio no encontrado");
                }

                if (servicio.Activo == "Y")
                {
                    return BadRequest("El servicio ya está activo");
                }

                var reactivated = await _db.ReactivateServicioAdicional(id);
                if (!reactivated)
                {
                    return StatusCode(500, "No se pudo reactivar el servicio");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al reactivar el servicio: " + ex.Message);
            }
        }
    }
}