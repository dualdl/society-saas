using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SocietySaaS.Functions;

public static class GenerateReceiptPdf
{
    [FunctionName("GenerateReceiptPdf")]
    public static async Task Run(
        [QueueTrigger("receipt-pdf-queue", Connection = "AzureStorage:ConnectionString")] string message,
        ILogger log)
    {
        log.LogInformation("Generating receipt PDF: {Message}", message);
        await Task.CompletedTask;
    }
}
