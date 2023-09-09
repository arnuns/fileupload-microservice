namespace FileUploadService.Models;

public class UploadResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}