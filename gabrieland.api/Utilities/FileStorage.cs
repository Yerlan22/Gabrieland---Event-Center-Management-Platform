// Services/FileStorageService.cs
using Microsoft.AspNetCore.Hosting;
using System.IO;

public class FileStorageService
{
    private readonly IWebHostEnvironment _env;
    private const string BaseUploadPath = "Uploads";
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveImage(IFormFile file, string subfolder)
    {
        // Validate file
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        if (file.Length > MaxFileSize)
            throw new ArgumentException($"File size exceeds {MaxFileSize / 1024 / 1024}MB limit");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type");

        // Create safe filename
        var sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName)
            .Replace(" ", "-", StringComparison.Ordinal)
            .ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}-{sanitizedFileName}{extension}";

        // Create directory if not exists
        var uploadPath = Path.Combine(_env.WebRootPath, BaseUploadPath, subfolder);
        Directory.CreateDirectory(uploadPath);

        // Save file
        var filePath = Path.Combine(uploadPath, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative path
        return $"/{BaseUploadPath}/{subfolder}/{uniqueFileName}";
    }

    public void DeleteImage(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            var physicalPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}