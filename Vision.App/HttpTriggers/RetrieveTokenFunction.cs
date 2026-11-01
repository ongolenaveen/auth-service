using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Vision.Data.Models;
using Vision.Services.Interfaces;
using Vision.Shared.Interfaces;
using Vision.Shared.Utilities;

namespace Vision.App.HttpTriggers;

public class RetrieveTokenFunction(IStorageService storageService, IEnvironmentConfig environmentConfig, ILogger<RetrieveTokenFunction> logger)
{
    [Function("RetrieveTokenFunction")]
    [OpenApiOperation(operationId: "RetrieveToken", tags: ["Authorize"], Description = "Retrieves a token stored in the persistence layer.")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "ApiKey", In = OpenApiSecurityLocationType.Header)]
    [OpenApiRequestBody("application/json", typeof(AuthCodeRequest), Description = "JSON request body containing { code, state}")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response message containing a JSON result.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ApiProblemDetails), Description = "Bad Request.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ApiProblemDetails), Description = "Internal Server Error.")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "token")] HttpRequestData req)
    {
        var targetCookie = req.Cookies.FirstOrDefault(c => c.Name == "KongSession");

        if (targetCookie == null)
        {
            logger.LogWarning("Session cookie is missing.");
            var redirectUrl = $"{environmentConfig.AzureAdBaseUri}{environmentConfig.AzureAdAuthorizeEndpoint}?response_type=code&redirect_uri={environmentConfig.AzureAdRedirectUri}&response_mode=form_post&state=zXiT-o0c_ICUFToFMxJIsr7V&nonce=-Uh5w4AlXth4c_9ONHp7HbIC&client_id={environmentConfig.AzureAdClientId}&scope={environmentConfig.AzureAdScope}";
            return new RedirectResult(redirectUrl);
        }

        var cookieValue = targetCookie.Value;
        logger.LogInformation($"Successfully read cookie value: {cookieValue}");

        var token = await storageService.ReadTokenAsync(cookieValue);

        var response = new { token?.AccessToken, token?.IdToken, token?.IssuedAt, token?.ExpiresAt };
        return new OkObjectResult(response);
    }
}