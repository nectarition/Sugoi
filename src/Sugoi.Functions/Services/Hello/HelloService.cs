using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using static Sugoi.Functions.Enumerations;
using Sugoi.Functions.Models.Interactions;
using Sugoi.Functions.Services.Common;

namespace Sugoi.Functions.Services;

public interface IHelloService
{
    Task<HttpResponseData> PongAsync(HttpRequestData req);
    Task<HttpResponseData> HelloWorldAsync(HttpRequestData req);
    Task<HttpResponseData> SugoiAsync(HttpRequestData req);
    Task<HttpResponseData> JomeiAsync(HttpRequestData req);
}

public class HelloService : IHelloService
{
    private ILogger Logger { get; }
    private IResponseService ResponseService { get; }

    public HelloService(
        IResponseService responseService,
        ILoggerFactory loggerFactory)
    {
        ResponseService = responseService;
        Logger = loggerFactory.CreateLogger<HelloService>();
    }

    public async Task<HttpResponseData> PongAsync(HttpRequestData req)
    {
        Logger.LogInformation("Pong!");

        return await ResponseService.ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.Pong
        });
    }

    public async Task<HttpResponseData> HelloWorldAsync(HttpRequestData req)
    {
        return await ResponseService.ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = "pong!"
            }
        });
    }

    public async Task<HttpResponseData> SugoiAsync(HttpRequestData req)
    {
        return await ResponseService.ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":woozy_face:"
            }
        });
    }

    public async Task<HttpResponseData> JomeiAsync(HttpRequestData req)
    {
        return await ResponseService.ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":weary:"
            }
        });
    }
}
