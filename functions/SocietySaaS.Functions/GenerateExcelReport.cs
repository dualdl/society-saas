using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SocietySaaS.Functions;

public static class GenerateExcelReport
{
    [FunctionName("GenerateExcelReport")]
    public static async Task Run(
        [QueueTrigger("report-queue", Connection = "AzureStorage:ConnectionString")] string message,
        ILogger log)
    {
        log.LogInformation("Generating Excel report: {Message}", message);
        await Task.CompletedTask;
    }
}
