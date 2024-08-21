using System.Net;
using System.Net.Mail;

namespace Comanda.WebApi.Services;

public sealed class SmtpEmailService(SmtpSettings settings, ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient(settings.Host)
        {
            Port = settings.Port,
            Credentials = new NetworkCredential(settings.UserName, settings.Password),
            EnableSsl = settings.UseSsl,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(settings.UserName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(to);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception exception)
        {
            logger.LogError("Failed to send email. Cause: {exception}", exception);
        }
    }
}