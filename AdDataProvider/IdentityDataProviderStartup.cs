using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Data.Interfaces;
using Vision.Shared.Interfaces;

namespace AdDataProvider
{
    public class IdentityDataProviderStartup : IDataProviderStartup
    {
        public void AddDataProviders(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var environmentConfig = serviceProvider.GetRequiredService<IEnvironmentConfig>();
            services.AddScoped<IIdentityDataProvider, IdentityDataProvider>();

            var azureAdBaseUri = environmentConfig.AzureAdBaseUri; 
            
            services.AddHttpClient("AzureAD", httpClient => 
            { 
                httpClient.BaseAddress = new Uri(azureAdBaseUri);
            });
        }
    }
}
