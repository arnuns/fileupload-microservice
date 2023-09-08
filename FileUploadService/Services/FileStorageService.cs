using FileUploadService.Entities;
using FileUploadService.Models;
using Microsoft.Extensions.Options;
using System.IO;

namespace FileUploadService.Services;

public interface IFileStorageService
{
    Task<FileUpload> UploadAsync(IFormFile file);
}

public class FileStorageService : IFileStorageService
{
    private readonly FileSettings _fileSettings;
    public FileStorageService(IOptions<FileSettings> options)
    {
        _fileSettings = options.Value;
    }

    public async Task<FileUpload> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length <= 0)
            throw new AppException("Invalid file format.");
        var fileExtension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(fileExtension))
            throw new AppException("Invalid file format.");
        if (ConvertBytesToMegabytes(file.Length) > _fileSettings.MaximumFileSize)
            throw new AppException($"The maximum size for upload is {_fileSettings.MaximumFileSize} MB.");
        return new FileUpload {};
    }

    private static double ConvertBytesToMegabytes(long bytes)
         => (bytes / 1024f) / 1024f;
}