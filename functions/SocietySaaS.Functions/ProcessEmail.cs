using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SocietySaaS.Functions;

public static class ProcessEmail
{
    [FunctionName("ProcessEmail")]
    public static async Task Run(
        [QueueTrigger("email-queue", Connection = "AzureStorage:ConnectionString")] string message,
        ILogger log)
    {
        log.LogInformation("Processing email: {Message}", message);
        await Task.CompletedTask;
    }
}
