using System.Net;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Vision.Data.Models;
using Vision.Services.Interfaces;
using Vision.Shared.Utilities;

namespace Vision.App.HttpTriggers;

public class CallbackFunction(IAuthorizationService authorizationService, ILogger<CallbackFunction> logger): BaseApiFunction
{
    [Function("CallbackFunction")]
    [OpenApiOperation(operationId: "Callback", tags: ["Authorize"], Description = "Callback request coming from Azure AD with auth code and state.")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "ApiKey", In = OpenApiSecurityLocationType.Header)]
    [OpenApiRequestBody("application/json", typeof(AuthCodeRequest), Description = "JSON request body containing { code, state}")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response message containing a JSON result.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ApiProblemDetails), Description = "Bad Request.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ApiProblemDetails), Description = "Internal Server Error.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "callback")] HttpRequestData req)
    {
        // 1. Read the raw form content as a string
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // 2. Parse the application/x-www-form-urlencoded string
        var formData = HttpUtility.ParseQueryString(requestBody);

        // 3. Extract your parameters
        var code = formData["code"];
        var state = formData["state"];

        if (string.IsNullOrWhiteSpace(code))
        {
            logger.LogWarning("Callback request is missing the required 'code' parameter.");

            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync("Authorization code missing.");
            return errorResponse;
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            logger.LogWarning("Callback request is missing the required 'state' parameter.");
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync("Authorization state missing.");
            return errorResponse;
        }

        var authCodeRequest = new AuthCodeRequest { Code = code, State = state };

        var (_, sessionId) = await authorizationService.ExchangeCodeForTokenAsync(authCodeRequest);

        var cookie = new HttpCookie("KongSession", sessionId)
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSite.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };

        var response = req.CreateResponse(HttpStatusCode.OK);

        response.Cookies.Append(cookie);

        await response.WriteStringAsync("Cookie successfully appended to response header!");

        return response;
    }
}