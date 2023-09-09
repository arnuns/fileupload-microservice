namespace FileUploadService.Models;

public class FileSettings
{
    public string Directory { get; set; } = "uploads";
    public int MaximumFileSize { get; set; } = 5;
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = default!;
    public string Topic { get; set; } = default!;
    public string GroupId { get; set; } = default!;
    public string AutoOffsetReset { get; set; } = default!;
    public string EnableAutoCommit { get; set; } = default!;
}