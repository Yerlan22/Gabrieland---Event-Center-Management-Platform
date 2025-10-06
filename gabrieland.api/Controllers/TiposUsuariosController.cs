// Controllers/TiposUsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using gabrieland.Data;
using gabrieland.api.Models;
using System.Threading.Tasks;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class TiposUsuariosController : ControllerBase
    {
        private readonly DataBaseConnector _db;

        public TiposUsuariosController(DataBaseConnector dataBaseConnector)
        {
            _db = dataBaseConnector;
        }

        // GET: /TiposUsuarios
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tiposUsuarios = await _db.GetTiposUsuarios();
            return Ok(tiposUsuarios);
        }

        // GET /TiposUsuarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tipoUsuario = await _db.GetTipoUsuarioById(id);
            if (tipoUsuario == null)
            {
                return NotFound();
            }
            return Ok(tipoUsuario);
        }

        // POST /TiposUsuarios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TiposUsuarios tipoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newId = await _db.CreateTipoUsuario(tipoUsuario);
                tipoUsuario.ID_Tipos_usuario = newId;

                return CreatedAtAction(nameof(Get), new { id = newId }, tipoUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al crear el tipo de usuario: " + ex.Message);
            }
        }

        // PUT /TiposUsuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TiposUsuarios tipoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _db.UpdateTipoUsuario(tipoUsuario);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar el tipo de usuario: " + ex.Message);
            }
        }

        // DELETE /TiposUsuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // First check if any users are using this type
                if (await _db.IsTipoUsuarioInUse(id))
                {
                    return BadRequest("No se puede eliminar, existen usuarios con este tipo");
                }

                var deleted = await _db.DeleteTipoUsuario(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al eliminar el tipo de usuario: " + ex.Message);
            }
        }
    }
}