using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Vision.App.Middleware;
using Vision.DI;

namespace Vision.App;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureFunctionsWebApplication(builder =>
            {
                builder.Services.AddVisionServices();
                builder.UseNewtonsoftJson();
                builder.UseMiddleware<ProblemDetailsMiddleware>();
            }).ConfigureServices(services =>
            {
                services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
                {
                    var options = new OpenApiConfigurationOptions
                    {
                        Info = new OpenApiInfo
                        {
                            Version = "1.0.0",
                            Title = "Vision API",
                            Description = "API to manage vision data.",
                            Contact = new OpenApiContact
                            {
                                Name = "Naveen Papisetty",
                                Email = "naveen.papisetty@outlook.com"
                            }
                        },
                        Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                        OpenApiVersion = OpenApiVersionType.V2,
                        IncludeRequestingHostName = true,
                        ForceHttps = false,
                        ForceHttp = false
                    };

                    return options;
                });
            });
        return builder;
    }
}