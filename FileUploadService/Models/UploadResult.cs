using FileUploadService.Entities;

namespace FileUploadService.Models;

public class UploadResult
{
    public bool IsSuccess { get; set; }
    public FileUpload? File { get; set; }
    public string? ErrorMessage { get; set; }
}