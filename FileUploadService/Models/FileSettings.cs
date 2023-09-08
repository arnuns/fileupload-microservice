namespace FileUploadService.Models;

public class FileSettings
{
    public string Directory { get; set; } = "uploads";
    public int MaximumFileSize { get; set; } = 5;
}