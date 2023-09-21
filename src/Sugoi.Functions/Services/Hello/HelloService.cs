using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using static Sugoi.Functions.Enumerations;
using Sugoi.Functions.Models.Interactions;
using Sugoi.Functions.Services.Common;
using Microsoft.Azure.Cosmos;

namespace Sugoi.Functions.Services;

public interface IHelloService
{
    Task<InteractionResult> HelloWorldAsync(InteractionPayload payload);
    Task<InteractionResult> SugoiAsync(InteractionPayload payload);
    Task<InteractionResult> JomeiAsync(InteractionPayload payload);
    Task<InteractionResult> DappunAsync(InteractionPayload payload);
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
                Content = "pong!"
            }
        };
    }

    public async Task<InteractionResult> SugoiAsync(InteractionPayload payload)
    {
        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":woozy_face:"
            }
        };
    }

    public async Task<InteractionResult> JomeiAsync(InteractionPayload payload)
    {
        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":weary:"
            }
        };
    }

    public async Task<InteractionResult> DappunAsync(InteractionPayload payload)
    {
        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = "あああああああああああああああああああああああああああああああ！！！！！！！！！！！（ﾌﾞﾘﾌﾞﾘﾌﾞﾘﾌﾞﾘｭﾘｭﾘｭﾘｭﾘｭﾘｭ！！！！！！ﾌﾞﾂﾁﾁﾌﾞﾌﾞﾌﾞﾁﾁﾁﾁﾌﾞﾘﾘｲﾘﾌﾞﾌﾞﾌﾞﾌﾞｩｩｩｩｯｯｯ！！！！！！！）"
            }
        };
    }
}
