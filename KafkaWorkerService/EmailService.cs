using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace KafkaWorkerService;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> options,
        ILogger<EmailService> logger)
    {
        _emailSettings = options.Value;
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
            // await client.SendMailAsync(mail);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending mail.");
            throw;
        }
    }
}