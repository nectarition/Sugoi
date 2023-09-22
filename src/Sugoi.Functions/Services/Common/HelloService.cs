using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Sugoi.Functions.Models.Interactions;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Services.Common;

public interface IHelloService
{
    Task<InteractionResult> HelloWorldAsync(InteractionPayload payload);
}

public class HelloService : IHelloService
{
    private ILogger Logger { get; }
    private IResponseService ResponseService { get; }
    private CosmosClient CosmosClient { get; }

    public HelloService(
        IResponseService responseService,
        CosmosClient cosmosClient,
        ILoggerFactory loggerFactory)
    {
        ResponseService = responseService;
        CosmosClient = cosmosClient;
        Logger = loggerFactory.CreateLogger<HelloService>();
    }

    public async Task<InteractionResult> HelloWorldAsync(InteractionPayload payload)
    {
        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":thinking:"
            }
        };
    }
}
