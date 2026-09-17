using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SocietySaaS.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var host = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        var port = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var username = _configuration["EmailSettings:Username"] ?? "";
        var password = _configuration["EmailSettings:Password"] ?? "";
        var fromEmail = _configuration["EmailSettings:FromEmail"] ?? username;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var message = new MailMessage(fromEmail, to, subject, htmlBody) { IsBodyHtml = true };
        await client.SendMailAsync(message);
    }
}
