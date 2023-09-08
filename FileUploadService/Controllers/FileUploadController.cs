using FileUploadService.Models;
using FileUploadService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmailService _emailService;
    public FileUploadController(IFileStorageService fileStorageService, IEmailService emailService)
    {
        _fileStorageService = fileStorageService;
        _emailService = emailService;

    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var result = await _fileStorageService.UploadAsync(file);
        if (result.IsSuccess)
        {
            await _emailService.SendAsync("recipient@example.com", "File Uploaded Successfully", "A new file has been uploaded to destination.");
            return Ok(result);
        }
        return BadRequest(result);
    }
}