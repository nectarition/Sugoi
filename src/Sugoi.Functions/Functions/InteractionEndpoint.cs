using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSec.Cryptography;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Models.Interactions;
using Sugoi.Functions.Services.Aggregates;
using Sugoi.Functions.Services.Common;
using System.Net;
using System.Text;
using System.Text.Json;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Functions;

public class InteractionEndpoint
{
    private ILogger Logger { get; }
    private SugoiConfiguration SugoiConfiguration { get; }
    private IResponseService ResponseService { get; }
    private IHelloService HelloService { get; }
    private IMessageService MessageService { get; }
    private IAggregateService AggregateService { get; }

    public InteractionEndpoint(
        ILoggerFactory loggerFactory,
        IOptions<SugoiConfiguration> sugoiConfigurationOptions,
        IResponseService responseService,
        IHelloService helloService,
        IMessageService messageService,
        IAggregateService aggregateService)
    {
        Logger = loggerFactory.CreateLogger<InteractionEndpoint>();
        SugoiConfiguration = sugoiConfigurationOptions.Value;
        ResponseService = responseService;
        HelloService = helloService;
        MessageService = messageService;
        AggregateService = aggregateService;
    }

    [Function(nameof(InteractionRoot))]
    public async Task<HttpResponseData> InteractionRoot([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        try
        {
            await VerifyRequestAsync(req);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Logger.LogError("トークンの検証に失敗しました。");

            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var interactionObject = await req.ReadFromJsonAsync<InteractionPayload>();
        if (interactionObject == null)
        {
            throw new NullReferenceException(nameof(interactionObject));
        }

        Logger.LogInformation($"Payload: {JsonSerializer.Serialize(interactionObject)}");

        if (interactionObject.InteractionType == InteractionTypes.Ping)
        {
            var noOpResult = await MessageService.PongAsync();
            return await ResponseService.ResponseCoreAsync(req, noOpResult);
        }

        Logger.LogInformation($"InteractionType: {interactionObject.InteractionType}");
        Logger.LogInformation($"CommandId: {interactionObject.Data?.Id ?? "<null>"}");
        Logger.LogInformation($"CommandName: {interactionObject.Data?.Name ?? "<null>"}");

        if (interactionObject.Data == null
            || !interactionObject.Data.Name.Contains("sugoi")
            || interactionObject.Data.Options.Length == 0)
        {
            var noOpResult = await MessageService.PongAsync();
            return await ResponseService.ResponseCoreAsync(req, noOpResult);
        }

        var option = interactionObject.Data.Options.FirstOrDefault();
        if (option == null)
        {
            var noOpResult = await MessageService.PongAsync();
            return await ResponseService.ResponseCoreAsync(req, noOpResult);
        }

        var result = option.Name switch
        {
            "ping" => await HelloService.HelloWorldAsync(interactionObject),
            "get-user" => await MessageService.GetUserByIdAsync(option),
            "set-username" => await MessageService.SetUserNameAsync(option),
            "set-username-byid" => await MessageService.SetUserNameAsync(option),
            //"create-user" => await MessageService.CreateUserAsync(option),
            //"delete-user" => await MessageService.DeleteUserAsync(option),
            "get-aggregate-result" => await MessageService.GetAggregateResult(),
            _ => throw new ArgumentOutOfRangeException(nameof(option))
        };

        return await ResponseService.ResponseCoreAsync(req, result);
    }

    private async Task VerifyRequestAsync(HttpRequestData req)
    {
        var signature = req.Headers.GetValues("X-Signature-Ed25519").First();
        if (string.IsNullOrEmpty(signature))
        {
            Logger.LogError("シグネチャの検証に失敗");
            throw new InvalidDataException(nameof(signature));
        }

        var reqTimestamp = req.Headers.GetValues("X-Signature-Timestamp").First();
        if (string.IsNullOrEmpty(reqTimestamp))
        {
            Logger.LogError("タイムスタンプの検証に失敗");
            throw new InvalidDataException(nameof(reqTimestamp));
        }

        var reqBody = await req.ReadAsStringAsync();
        if (string.IsNullOrEmpty(reqBody))
        {
            Logger.LogError("リクエストボディの検証に失敗");
            throw new InvalidDataException(nameof(reqBody));
        }

        Logger.LogInformation($"timebody: {reqTimestamp}{reqBody}");
        Logger.LogInformation($"sig: {signature}");

        var algorithm = SignatureAlgorithm.Ed25519;

        var signatureBytes = Convert.FromHexString(signature);
        var signingBodyBytes = Encoding.UTF8.GetBytes($"{reqTimestamp}{reqBody}");

        var publicKeyBytes = Convert.FromHexString(SugoiConfiguration.Secrets.BotPublicKey);
        var publicKey = PublicKey.Import(algorithm, publicKeyBytes, KeyBlobFormat.RawPublicKey);

        if (!algorithm.Verify(publicKey, signingBodyBytes, signatureBytes))
        {
            Logger.LogError("署名の検証に失敗");
            throw new InvalidDataException(nameof(algorithm.Verify));
        }

        req.Body.Seek(0, SeekOrigin.Begin);
    }
}
