using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSec.Cryptography;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Models;
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
            Logger.LogInformation("Pong!");
            return await PingAsync(req);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);

        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString("Welcome to Azure Functions!");

        Logger.LogInformation("関数の実行に成功しました。");
        return response;
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

    private static async Task<HttpResponseData> PingAsync(HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        var result = new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.Pong
        };

        await res.WriteAsJsonAsync(result);

        return res;
    }
}
