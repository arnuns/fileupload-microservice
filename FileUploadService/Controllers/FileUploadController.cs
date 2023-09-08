using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    public FileUploadController() {}

    [HttpPost]
    public IActionResult UploadFile(IFormFile file)
    {
        if (file == null || file.Length <= 0)
            return BadRequest("Invalid file");
        return Ok("File uploaded successfully");
    }
}