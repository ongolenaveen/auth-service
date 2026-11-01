using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Vision.Shared.Interfaces;
using Vision.Shared.Utilities;

namespace Vision.App.HttpTriggers
{
    public class AuthorizeFunction(IEnvironmentConfig environmentConfig, ILogger<AuthorizeFunction> logger) : BaseApiFunction
    {
        [Function("AuthorizationFunction")]
        [OpenApiOperation(operationId: "Authorize", tags: ["Authorize"], Description = "Authorize request based on provided credentials.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "ApiKey", In = OpenApiSecurityLocationType.Header)]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Authorization Successful")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ApiProblemDetails), Description = "Bad Request.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ApiProblemDetails), Description = "Internal Server Error.")]
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "authorize")] HttpRequestData req)
        {
            logger.LogInformation("Starting authorization.");
            var targetCookie = req.Cookies.FirstOrDefault(c => c.Name == "KongSession");

            if (targetCookie == null)
            {
                logger.LogWarning("Session cookie is missing.");
                var redirectUrl = $"{environmentConfig.AzureAdBaseUri}{environmentConfig.AzureAdAuthorizeEndpoint}?response_type=code&redirect_uri={environmentConfig.AzureAdRedirectUri}&response_mode=form_post&state=zXiT-o0c_ICUFToFMxJIsr7V&nonce=-Uh5w4AlXth4c_9ONHp7HbIC&client_id={environmentConfig.AzureAdClientId}&scope={environmentConfig.AzureAdScope}";
                var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
                redirectResponse.Headers.Add("Location", redirectUrl);
                return redirectResponse;
            }

            var cookieValue = targetCookie.Value;
            logger.LogInformation($"Successfully read cookie value: {cookieValue}");

            var response = req.CreateResponse(HttpStatusCode.OK);

            response.WriteString("Session cookie is present in the request header!");
            return response;
        }
    }
}
