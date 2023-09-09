using Confluent.Kafka;
using FileUploadService.Models;
using Microsoft.Extensions.Options;

namespace FileUploadService.Services;

public interface IFileStorageService
{
    Task<UploadResult> UploadAsync(IFormFile file);
}

public class FileStorageService : IFileStorageService
{
    private readonly FileSettings _fileSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<FileStorageService> _logger;
    public FileStorageService(
        IOptions<FileSettings> fileSettings,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<FileStorageService> logger)
    {
        _fileSettings = fileSettings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
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
            var tempFilePath = Path.Combine(path, uniqueFileName);
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var producerConfig = new ProducerConfig { BootstrapServers = _kafkaSettings.BootstrapServers };
            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            await producer.ProduceAsync(_kafkaSettings.Topic, new Message<Null, string> { Value = tempFilePath });
            producer.Flush(TimeSpan.FromSeconds(10));

            return new UploadResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            var errorMessage = "An error occurred while uploading file.";
            _logger.LogError(ex, errorMessage);
            return new UploadResult { ErrorMessage = errorMessage};
        }
    }

    private static double ConvertBytesToMegabytes(long bytes)
         => (bytes / 1024f) / 1024f;
}