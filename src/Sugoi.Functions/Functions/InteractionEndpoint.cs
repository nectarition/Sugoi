using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSec.Cryptography;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Models.Interactions;
using Sugoi.Functions.Services;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Functions;

public class InteractionEndpoint
{
    private ILogger Logger { get; }
    private SugoiConfiguration SugoiConfiguration { get; }
    private IHelloService HelloService { get; }

    public InteractionEndpoint(
        ILoggerFactory loggerFactory,
        IOptions<SugoiConfiguration> sugoiConfigurationOptions,
        IHelloService helloService)
    {
        Logger = loggerFactory.CreateLogger<InteractionEndpoint>();
        SugoiConfiguration = sugoiConfigurationOptions.Value;
        HelloService = helloService;
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
        if (interactionObject == null) {
            throw new NullReferenceException(nameof(interactionObject));
        }

        if (interactionObject.InteractionType == InteractionTypes.Ping)
        {
            return await HelloService.PongAsync(req);
        }

        Logger.LogInformation($"InteractionType: {interactionObject.InteractionType}");
        Logger.LogInformation($"CommandId: {interactionObject.Data?.Id ?? "<null>"}");
        Logger.LogInformation($"CommandName: {interactionObject.Data?.Name ?? "<null>" }");

        if (interactionObject.Data == null
            || !interactionObject.Data.Name.Contains("sugoi")
            || interactionObject.Data.Options.Length == 0)
        {
            return await HelloService.PongAsync(req);
        }

        var option = interactionObject.Data.Options.FirstOrDefault();
        if (option == null)
        {
            return await HelloService.PongAsync(req);
        }

        switch (option.Name)
        {
            case "ping":
                return await HelloService.HelloWorldAsync(req);

            case "sugoi":
                return await HelloService.SugoiAsync(req);

            case "jomei":
                return await HelloService.JomeiAsync(req);

            default:
                return await HelloService.PongAsync(req);
        }
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
