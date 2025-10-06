// Controllers/FotosSalasController.cs
using gabrieland.api.Models;
using gabrieland.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("Salas/Fotos")]
public class FotosSalasController : ControllerBase
{
    private readonly DataBaseConnector _db;
    private readonly FileStorageService _fileStorage;
    private readonly ILogger<FotosSalasController> _logger;

    public FotosSalasController(DataBaseConnector db, FileStorageService fileStorage,
                               ILogger<FotosSalasController> logger)
    {
        _db = db;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    [HttpGet("todas")]
    public async Task<IActionResult> GetTodasLasFotos()
    {
        var fotos = await _db.GetTodasLasFotosAsync();
        return Ok(fotos);
    }

    [HttpGet("{salaId}")]
    public async Task<IActionResult> GetBySala(int salaId)
    {
        var fotos = await _db.GetFotosBySalaId(salaId);
        return Ok(fotos);
    }

    [HttpPost("{salaId}")]
    public async Task<IActionResult> Upload(int salaId, IFormFile file)
    {
        try
        {
            // Save file to disk
            var filePath = await _fileStorage.SaveImage(file, "salas");

            Console.WriteLine(salaId);

            // Save to database
            var fotoId = await _db.AddFotoSala(new FotoSala
            {
                foto = filePath,
                SAL_ID_Sala = salaId
            });

            return CreatedAtAction(nameof(GetBySala), new { salaId }, new
            {
                Id = fotoId,
                Path = filePath
            });
        }
        catch (ArgumentException ex)
        {   
            Console.WriteLine("argument");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {   
            Console.WriteLine("idk");
            _logger.LogError(ex, "Error uploading sala photo");
            return StatusCode(500, "Error uploading photo");
        }
    }

    [HttpDelete("{fotoId}")]
    public async Task<IActionResult> Delete(int fotoId)
    {
        try
        {
            // First get the photo to delete the file
            var foto = await _db.GetSalaFotobyId(fotoId);

            if (foto == null)
                return NotFound();

            // Delete file from storage
            _fileStorage.DeleteImage(foto.foto);

            // Delete from database
            var deleted = await _db.DeleteFotoSala(fotoId);

            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sala photo");
            return StatusCode(500, "Controller Error deleting photo: " + ex.Message);
        }
    }
}