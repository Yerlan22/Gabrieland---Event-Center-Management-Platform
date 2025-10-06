// UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using gabrieland.Data;
using gabrieland.api.Models;
using System.Threading.Tasks;

namespace gabrieland.api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataBaseConnector _db;

        public UsuariosController(DataBaseConnector dataBaseConnector)
        {
            _db = dataBaseConnector;
        }

        // GET: /Usuarios
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _db.GetUsuarios(incluirInactivos);
            return Ok(usuarios);
        }

        // GET /Usuarios/5?incluirInactivos=true
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery] bool incluirInactivos = false)
        {
            var usuario = await _db.GetUsuarioById(id, incluirInactivos);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
        // POST /Usuarios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                usuario.Activo = "Y"; // Ensure new users are active by default
                var newId = await _db.CreateUsuario(usuario);
                usuario.ID_Usuario = newId;
                usuario.Hash_Contraseña = "";

                return CreatedAtAction(nameof(Get), new { id = newId }, usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al crear el usuario:" + ex.Message);
            }
        }

        // PUT /Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.ID_Usuario)
            {
                return BadRequest("ID del usuario no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _db.UpdateUsuario(usuario);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar el usuario: " + ex.Message);
            }
        }

        // DELETE /Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // First check if user exists
                var usuario = await _db.GetUsuarioById(id);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                // Check if already inactive
                if (usuario.Activo == "N")
                {
                    return BadRequest("El usuario ya está inactivo");
                }

                var deactivated = await _db.LogicalDeleteUsuario(id);
                if (!deactivated)
                {
                    return StatusCode(500, "No se pudo desactivar el usuario");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al desactivar el usuario:" + ex.Message);
            }
        }
        // Reactivate /5/reactivar
        [HttpPatch("{id}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            try
            {
                var usuario = await _db.GetUsuarioById(id, true);
                if (usuario == null) return NotFound("Usuario no encontrado");
                if (usuario.Activo == "Y") return BadRequest("El usuario ya está activo");

                var updated = await _db.ReactivateUsuario(id);
                return updated ? NoContent() : StatusCode(500, "No se pudo reactivar");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }
    }
}