using Azure;
using Azure.AI.Vision.Face;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Data.Interfaces;
using Vision.Shared.Interfaces;

namespace FaceDetection.DataProvider
{
    public class FaceDetectionDataProviderStartup : IDataProviderStartup
    {
        public void AddDataProviders(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var environmentConfig = serviceProvider.GetRequiredService<IEnvironmentConfig>();
            services.AddScoped<IFaceDetectionDataProvider, FaceDetectionDataProvider>();

            services.AddSingleton((_) =>
            {
                var apiKey = environmentConfig.FaceKey;
                var faceEndpoint = environmentConfig.FaceEndpoint;

                var faceClient = new FaceClient(new Uri(faceEndpoint), new AzureKeyCredential(apiKey));

                return faceClient;
            });

            services.AddSingleton((_) =>
            {
                var apiKey = environmentConfig.FaceKey;
                var faceEndpoint = environmentConfig.FaceEndpoint;

                var faceAdministrationClient = new FaceAdministrationClient(new Uri(faceEndpoint), new AzureKeyCredential(apiKey));

                return faceAdministrationClient;
            });
        }
    }
}
