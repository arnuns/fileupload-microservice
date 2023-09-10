namespace KafkaWorkerService;

public class FileUploadMessage
{
    public string? TempFilePath { get; set; }
    public int? UserId { get; set; }
}