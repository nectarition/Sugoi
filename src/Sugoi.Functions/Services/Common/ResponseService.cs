using Microsoft.Azure.Functions.Worker.Http;
using Sugoi.Functions.Models.Interactions;

namespace Sugoi.Functions.Services.Common;

public interface IResponseService
{
    Task<HttpResponseData> ResponseCoreAsync(HttpRequestData req, InteractionResult result);
}

public class ResponseService : IResponseService
{
    public async Task<HttpResponseData> ResponseCoreAsync(HttpRequestData req, InteractionResult result)
    {
        var res = req.CreateResponse();
        await res.WriteAsJsonAsync(result);

        return res;
    }
}
