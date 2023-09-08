namespace FileUploadService.Models;

public class EmailSettings
{
    public string MailServer { get; set; } = default!;
    public int MailPort { get; set; }
    public string SenderName { get; set; } = default!;
    public string Sender { get; set; } = default!;
}