using Azure;
using Azure.AI.ContentSafety;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Data.Interfaces;
using Vision.Shared.Interfaces;

namespace ContentCheck.DataProvider
{
    public class ContentCheckDataProviderStartup : IDataProviderStartup
    {
        public void AddDataProviders(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var environmentConfig = serviceProvider.GetRequiredService<IEnvironmentConfig>();
            services.AddScoped<IContentCheckDataProvider, ContentCheckDataProvider>();

            services.AddSingleton((_) =>
            {
                var apiKey = environmentConfig.ContentSafetyKey;
                var contentSafetyEndpoint = environmentConfig.ContentSafetyEndpoint;

                var contentSafetyClient = new ContentSafetyClient(new Uri(contentSafetyEndpoint), new AzureKeyCredential(apiKey));

                return contentSafetyClient;
            });
        }
    }

}
