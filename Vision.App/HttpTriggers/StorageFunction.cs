using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Vision.Data.Models;
using Vision.Services.Interfaces;
using Vision.Shared.Utilities;

namespace Vision.App.HttpTriggers;

public class StorageFunction(IStorageService storageService, ILogger<StorageFunction> logger)
{
    [Function("StorageFunction")]
    [OpenApiOperation(operationId: "RetrieveTokenFromStorage", tags: ["Storage"],
        Description = "Retrieves a token stored in the persistence layer.")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "ApiKey",
        In = OpenApiSecurityLocationType.Header)]
    [OpenApiRequestBody("application/json", typeof(UserToken),
        Description = "JSON request body containing the user token details")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string),
        Description = "The OK response message containing a JSON result.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(ApiProblemDetails), Description = "Bad Request.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json",
        bodyType: typeof(ApiProblemDetails), Description = "Internal Server Error.")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "tokens")] HttpRequest req)
    {
        var reader = new StreamReader(req.Body);
        var requestContent = await reader.ReadToEndAsync();
        logger.LogInformation(requestContent);
        var userToken = JsonConvert.DeserializeObject<UserToken>(requestContent);
        await storageService.CreateTokenAsync(userToken);
        return new OkObjectResult("Token created successfully.");
    }
}