using FileUploadService.Services;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var result = await _fileStorageService.UploadAsync(file);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }
}