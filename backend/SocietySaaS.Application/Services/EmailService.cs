using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IEmailService
{
    Task QueueEmailAsync(string toEmail, string subject, string templateName, Dictionary<string, string> parameters);
    Task SendOtpEmailAsync(string toEmail, string code);
    Task SendWelcomeEmailAsync(string toEmail, string societyName, string adminName);
    Task SendBillEmailAsync(string toEmail, string flatNumber, decimal amount, string billingPeriod, string dueDate);
    Task SendReceiptEmailAsync(string toEmail, string receiptNumber, decimal amount, string flatNumber);
}

public class EmailService : IEmailService
{
    private readonly IEmailQueueRepository _emailQueueRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository;

    public EmailService(IEmailQueueRepository emailQueueRepository, IEmailTemplateRepository emailTemplateRepository)
    {
        _emailQueueRepository = emailQueueRepository;
        _emailTemplateRepository = emailTemplateRepository;
    }

    public async Task QueueEmailAsync(string toEmail, string subject, string templateName, Dictionary<string, string> parameters)
    {
        var template = await _emailTemplateRepository.GetByNameAsync(templateName);
        var body = template?.Body ?? "";
        foreach (var param in parameters)
        {
            body = body.Replace($"{{{{{param.Key}}}}}", param.Value);
        }

        var email = new EmailQueue
        {
            ToEmail = toEmail, Subject = subject, Body = body,
            TemplateName = templateName, Status = "Pending"
        };
        await _emailQueueRepository.AddAsync(email);
    }

    public async Task SendOtpEmailAsync(string toEmail, string code)
    {
        await QueueEmailAsync(toEmail, "Your OTP Code", "OTP", new Dictionary<string, string> { { "Code", code } });
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string societyName, string adminName)
    {
        await QueueEmailAsync(toEmail, $"Welcome to {societyName}", "Welcome",
            new Dictionary<string, string> { { "SocietyName", societyName }, { "AdminName", adminName } });
    }

    public async Task SendBillEmailAsync(string toEmail, string flatNumber, decimal amount, string billingPeriod, string dueDate)
    {
        await QueueEmailAsync(toEmail, $"Bill for {flatNumber} - {billingPeriod}", "Bill",
            new Dictionary<string, string> { { "FlatNumber", flatNumber }, { "Amount", amount.ToString("N2") }, { "BillingPeriod", billingPeriod }, { "DueDate", dueDate } });
    }

    public async Task SendReceiptEmailAsync(string toEmail, string receiptNumber, decimal amount, string flatNumber)
    {
        await QueueEmailAsync(toEmail, $"Payment Receipt {receiptNumber}", "Receipt",
            new Dictionary<string, string> { { "ReceiptNumber", receiptNumber }, { "Amount", amount.ToString("N2") }, { "FlatNumber", flatNumber } });
    }
}
