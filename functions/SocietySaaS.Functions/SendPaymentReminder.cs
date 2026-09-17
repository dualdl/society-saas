using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SocietySaaS.Functions;

public static class SendPaymentReminder
{
    [FunctionName("SendPaymentReminder")]
    public static async Task Run(
        [TimerTrigger("0 0 9 * * *")] TimerInfo timer,
        ILogger log)
    {
        log.LogInformation("Running payment reminder job at {Time}", DateTime.UtcNow);
        await Task.CompletedTask;
    }
}
