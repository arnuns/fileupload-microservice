namespace KafkaWorkerService;

public class EmailSettings
{
    public string MailServer { get; set; } = default!;
    public int MailPort { get; set; }
    public string SenderName { get; set; } = default!;
    public string Sender { get; set; } = default!;
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = default!;
    public string Topic { get; set; } = default!;
    public string GroupId { get; set; } = default!;
    public string AutoOffsetReset { get; set; } = default!;
    public string EnableAutoCommit { get; set; } = default!;
}

public class AWSSettings
{
    public string BucketName { get; set; } = default!;
}