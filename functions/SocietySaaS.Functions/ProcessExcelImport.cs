using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SocietySaaS.Functions;

public static class ProcessExcelImport
{
    [FunctionName("ProcessExcelImport")]
    public static async Task Run(
        [QueueTrigger("import-queue", Connection = "AzureStorage:ConnectionString")] string message,
        ILogger log)
    {
        log.LogInformation("Processing Excel import: {Message}", message);
        await Task.CompletedTask;
    }
}
