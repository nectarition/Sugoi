using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Sugoi.Functions.Functions;
public class AggregateMessages
{
    private ILogger Logger { get; }

    public AggregateMessages(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<AggregateMessages>();
    }

    [Function(nameof(AggregateMessagesWithHttp))]
    public HttpResponseData AggregateMessagesWithHttp([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        Logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;
    }

    [Function(nameof(AggregateMessagesWithTimer))]
    public void AggregateMessagesWithTimer([TimerTrigger("* * * * * *")] TimerInfo myTimer)
    {
        if (myTimer.IsPastDue)
        {
            Logger.LogInformation("Timer is running late!");
        }

        Logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    }
}
