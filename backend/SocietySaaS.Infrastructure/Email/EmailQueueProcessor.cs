using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Email;

public class EmailQueueProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailQueueProcessor> _logger;

    public EmailQueueProcessor(IServiceProvider serviceProvider, ILogger<EmailQueueProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var emailQueueRepository = scope.ServiceProvider.GetRequiredService<IEmailQueueRepository>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var pendingEmails = await emailQueueRepository.GetPendingEmailsAsync(10);
                foreach (var email in pendingEmails)
                {
                    try
                    {
                        await emailSender.SendAsync(email.ToEmail, email.Subject, email.Body);
                        email.Status = "Sent";
                        email.SentAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send email to {Email}", email.ToEmail);
                        email.RetryCount++;
                        email.ErrorMessage = ex.Message;
                        if (email.RetryCount >= 3) email.Status = "Failed";
                    }
                    await emailQueueRepository.UpdateAsync(email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email queue processor error");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
