using gabrieland.api.Models;
using gabrieland.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("Servicios/Fotos")]
public class FotosServiciosController : ControllerBase
{
    private readonly DataBaseConnector _db;
    private readonly FileStorageService _fileStorage;
    private readonly ILogger<FotosSalasController> _logger;

    public FotosServiciosController(DataBaseConnector db, FileStorageService fileStorage,
                               ILogger<FotosSalasController> logger)
    {
        _db = db;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    [HttpGet("todasservicios")]
    public async Task<IActionResult> GetTodasLasFotosServicios()
    {
        var fotos = await _db.GetTodasLasFotosServiciosAsync();
        return Ok(fotos);
    }

    [HttpGet("{servicioId}")]
    public async Task<IActionResult> GetByServicio(int servicioId)
    {
        var fotos = await _db.GetFotosByServicioId(servicioId);
        return Ok(fotos);
    }

    [HttpPost("{servicioId}")]
    public async Task<IActionResult> Upload(int servicioId, IFormFile file)
    {
        try
        {
            // Save file to disk
            var filePath = await _fileStorage.SaveImage(file, "servicios");

            // Save to database
            var fotoId = await _db.AddFotoServicio(new FotoServicio
            {
                foto = filePath,
                SAL_ID_Servicio = servicioId
            });

            return CreatedAtAction(nameof(GetByServicio), new { servicioId }, new
            {
                Id = fotoId,
                Path = filePath
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading servicios photo");
            return StatusCode(500, "Error uploading photo:" + ex.StackTrace);
        }
    }

    [HttpDelete("{fotoId}")]
    public async Task<IActionResult> Delete(int fotoId)
    {
        try
        {
            // First get the photo to delete the file
            var foto = await _db.GetServicioFotobyId(fotoId);

            if (foto == null)
                return NotFound();

            // Delete file from storage
            _fileStorage.DeleteImage(foto.foto);

            // Delete from database
            var deleted = await _db.DeleteFotoServicio(fotoId);

            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sala photo");
            return StatusCode(500, "Controller Error deleting photo: " + ex.Message);
        }
    }
}