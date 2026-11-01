using System.Net;
using Microsoft.AspNetCore.Http;

namespace Vision.App.Extensions
{
    public static class HttpRequestExtensions
    {
        public static HttpResponse CreateResponse(this HttpRequest req, HttpStatusCode statusCode)
        {
            // Grab the response lifecycle context attached to the current request
            var response = req.HttpContext.Response;
            response.StatusCode = (int)statusCode;
            return response;
        }
    }
}
