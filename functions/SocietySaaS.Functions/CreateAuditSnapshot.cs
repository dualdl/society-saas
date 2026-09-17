using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SocietySaaS.Functions;

public static class CreateAuditSnapshot
{
    [FunctionName("CreateAuditSnapshot")]
    public static async Task Run(
        [TimerTrigger("0 0 0 1 1 *")] TimerInfo timer,
        ILogger log)
    {
        log.LogInformation("Creating audit snapshot at {Time}", DateTime.UtcNow);
        await Task.CompletedTask;
    }
}
