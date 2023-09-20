using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSec.Cryptography;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Models.Interactions;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Functions;

public class InteractionEndpoint
{
    private ILogger Logger { get; }
    private SugoiConfiguration SugoiConfiguration { get; }

    public InteractionEndpoint(
        ILoggerFactory loggerFactory,
        IOptions<SugoiConfiguration> sugoiConfigurationOptions)
    {
        Logger = loggerFactory.CreateLogger<InteractionEndpoint>();
        SugoiConfiguration = sugoiConfigurationOptions.Value;
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
            return await PongAsync(req);
        }

        Logger.LogInformation($"InteractionType: {interactionObject.InteractionType}");
        Logger.LogInformation($"CommandId: {interactionObject.Data?.Id ?? "<null>"}");
        Logger.LogInformation($"CommandName: {interactionObject.Data?.Name ?? "<null>" }");

        if (interactionObject.Data == null
            || !interactionObject.Data.Name.Contains("sugoi")
            || interactionObject.Data.Options.Length == 0)
        {
            return await PongAsync(req);
        }

        var option = interactionObject.Data.Options.FirstOrDefault();
        if (option == null)
        {
            return await PongAsync(req);
        }

        switch (option.Name)
        {
            case "ping":
                return await HelloWorldAsync(req);

            case "sugoi":
                return await SugoiAsync(req);

            case "jomei":
                return await JomeiAsync(req);

            default:
                return await PongAsync(req);
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

    private async Task<HttpResponseData> ResponseCoreAsync(HttpRequestData req, InteractionResult result)
    {
        var res = req.CreateResponse();
        await res.WriteAsJsonAsync(result);

        return res;
    }

    private async Task<HttpResponseData> PongAsync(HttpRequestData req)
    {
        Logger.LogInformation("Pong!");

        return await ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.Pong
        });
    }

    private async Task<HttpResponseData> HelloWorldAsync(HttpRequestData req)
    {
        return await ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = "pong!"
            }
        });
    }

    private async Task<HttpResponseData> SugoiAsync(HttpRequestData req)
    {
        return await ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":woozy_face:"
            }
        });
    }

    private async Task<HttpResponseData> JomeiAsync(HttpRequestData req)
    {
        return await ResponseCoreAsync(req, new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new InteractionResultData
            {
                Content = ":weary:"
            }
        });
    }
}
