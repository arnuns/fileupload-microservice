using FileUploadService.Entities;
using FileUploadService.Entities.Repositories;
using FileUploadService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;


namespace FileUploadService.Services;

public interface IFileStorageService
{
    Task<UploadResult> UploadAsync(IFormFile file);
}

public class FileStorageService : IFileStorageService
{
    private readonly FileSettings _fileSettings;
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly ILogger<FileStorageService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public FileStorageService(
        IOptions<FileSettings> options,
        IFileUploadRepository fileUploadRepository,
        ILogger<FileStorageService> logger,
        IUnitOfWork unitOfWork)
    {
        _fileSettings = options.Value;
        _fileUploadRepository = fileUploadRepository;
        _logger = logger;

        _unitOfWork = unitOfWork;
    }

    public async Task<UploadResult> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length <= 0)
            throw new AppException("Invalid file format.");
        var fileExtension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(fileExtension))
            throw new AppException("Invalid file extension.");
        if (ConvertBytesToMegabytes(file.Length) > _fileSettings.MaximumFileSize)
            throw new AppException("File size exceeds the allowed limit.");
        try
        {
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, _fileSettings.Directory));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var uniqueFileName = $"{Guid.NewGuid().ToString("N")}_{file.FileName}";
            var filePath = Path.Combine(path, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var fileUpload = new FileUpload 
            {
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await _fileUploadRepository.AddAsync(fileUpload);
            await _unitOfWork.CompleteAsync();
            return new UploadResult 
            {
                IsSuccess = true,
                File = fileUpload
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading the file.");
            return new UploadResult {};
        }
    }

    private static double ConvertBytesToMegabytes(long bytes)
         => (bytes / 1024f) / 1024f;
}