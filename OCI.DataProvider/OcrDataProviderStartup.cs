using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Data.Interfaces;
using Vision.Shared.Interfaces;

namespace Ocr.DataProvider
{
    public class OcrDataProviderStartup : IDataProviderStartup
    {
        public void AddDataProviders(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var environmentConfig = serviceProvider.GetRequiredService<IEnvironmentConfig>();
            services.AddScoped<IOcrDataProvider, OcrDataProvider>();

            services.AddSingleton((_) =>
            {
                var apiKey = environmentConfig.VisionKey;
                var visionEndpoint = environmentConfig.VisionEndpoint;

                var imageAnalysisClient = new ImageAnalysisClient(new Uri(visionEndpoint), new AzureKeyCredential(apiKey));

                return imageAnalysisClient;
            });
        }
    }
}
