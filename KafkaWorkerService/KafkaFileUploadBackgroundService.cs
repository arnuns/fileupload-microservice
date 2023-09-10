using Confluent.Kafka;
using KafkaWorkerService.Entities;
using KafkaWorkerService.Entities.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaWorkerService;

public class KafkaFileUploadBackgroundService : BackgroundService
{
    private readonly IDbContextFactory<FileUploadContext> _contextFactory;
    private readonly KafkaSettings _kafkaSettings;
    private readonly AWSSettings _awsSettings;
    private readonly ILogger<KafkaFileUploadBackgroundService> _logger;
    private readonly IEmailService _emailService;

    public KafkaFileUploadBackgroundService(
        IDbContextFactory<FileUploadContext> contextFactory,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<AWSSettings> awsSettings,
        ILogger<KafkaFileUploadBackgroundService> logger,
        IEmailService emailService)
    {
        _contextFactory = contextFactory;
        _kafkaSettings = kafkaSettings.Value;
        _awsSettings = awsSettings.Value;
        _logger = logger;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = (AutoOffsetReset)Enum.Parse(typeof(AutoOffsetReset), _kafkaSettings.AutoOffsetReset, true),
            EnableAutoCommit = bool.Parse(_kafkaSettings.EnableAutoCommit)
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_kafkaSettings.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var messageObject = JsonConvert.DeserializeObject<FileUploadMessage>(consumeResult.Message.Value);
                var tempFilePath = messageObject?.TempFilePath;
                if (messageObject?.UserId == null)
                    throw new ArgumentNullException("UserId is not set.");
                var userId = messageObject.UserId.GetValueOrDefault();
                if (File.Exists(tempFilePath))
                {
                    // var s3BucketName = _awsSettings.BucketName;
                    var s3Key = Path.GetFileName(tempFilePath);
                    var filePath = $"{_awsSettings.BucketName}/{s3Key}";

                    using var fileStream = new FileStream(tempFilePath, FileMode.Open);
                    long fileLength = fileStream.Length;
                    // var putRequest = new PutObjectRequest
                    // {
                    //     BucketName = s3BucketName,
                    //     Key = s3Key,
                    //     InputStream = fileStream
                    // };

                    // await _s3Client.PutObjectAsync(putRequest, stoppingToken);

                    _logger.LogInformation($"Temp: {tempFilePath}, UserId: {userId}");
                    _logger.LogInformation("The file has been successfully uploaded to S3.");

                    using var context = _contextFactory.CreateDbContext();
                    var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
                    var fileUpload = new FileUpload
                    {
                        UserId = user.Id,
                        FileName = s3Key,
                        FilePath = filePath,
                        FileSize = fileLength,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };
                    _logger.LogInformation(fileUpload.FilePath);
                    context.FileUpload.Add(fileUpload);
                    var affectedRows = await context.SaveChangesAsync();
                    if (affectedRows == 0)
                        _logger.LogInformation("No changes detected in the DbContext.");
                    _logger.LogInformation("The file has been successfully saved to DB.");

                    File.Delete(tempFilePath);
                    _logger.LogInformation("The temporary file has been deleted.");

                    await _emailService.SendAsync(user.Email, "File Upload Complete", $"Your file has been successfully processed. ({filePath})");
                    _logger.LogInformation("Email notification has been sent.");
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "An error occurred while kafka processing a file.");
            }
        }

        consumer.Close();
    }
}
