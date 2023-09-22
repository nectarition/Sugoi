using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Sugoi.Functions.Services.Aggregates;

namespace Sugoi.Functions.Functions;

public class AggregateMessages
{
    private ILogger Logger { get; }
    private IAggregateService AggregateService { get; }

    public AggregateMessages(
        ILoggerFactory loggerFactory,
        IAggregateService aggregateService)
    {
        AggregateService = aggregateService;
        Logger = loggerFactory.CreateLogger<AggregateMessages>();
    }

    [Function(nameof(AggregateMessagesWithTimerAsync))]
    public async Task AggregateMessagesWithTimerAsync(
        [TimerTrigger("0 0 3 * * *")] TimerInfo myTimer,
        [DurableClient] DurableTaskClient client)
    {
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrchestrationAggregateAsync));
        Logger.LogInformation($"InstanceId: {instanceId}");
    }

    [Function(nameof(ManualAggregateAsync))]
    public async Task<HttpResponseData> ManualAggregateAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrchestrationAggregateAsync));
        var result = client.CreateCheckStatusResponse(req, instanceId);

        return result;
    }

    [Function(nameof(OrchestrationAggregateAsync))]
    public async Task OrchestrationAggregateAsync([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        await context.CallActivityAsync(nameof(AggregateCoreAsync));
    }

    [Function(nameof(AggregateCoreAsync))]
    public async Task AggregateCoreAsync([ActivityTrigger] string input)
    {
        Logger.LogInformation("----- Started Aggregation");
        await AggregateService.AggregateMessages();
        Logger.LogInformation("----- Complete Aggregation!");
    }
}
