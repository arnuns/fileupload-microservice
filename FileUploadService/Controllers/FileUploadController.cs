using FileUploadService.Models;
using FileUploadService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    public FileUploadController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var result = await _fileStorageService.UploadAsync(file);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}