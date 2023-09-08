using FileUploadService.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace FileUploadService.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<FileStorageService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<FileStorageService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            using var mail = new MailMessage(_emailSettings.Sender, to)
            {
                Subject = subject,
                Body = body
            };
            using var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Sender, smtpPassword),
                EnableSsl = true
            };
            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending mail.");
        }
    }
}