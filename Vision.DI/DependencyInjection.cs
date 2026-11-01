using AdDataProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.DataProvider;
using Vision.Services;
using Vision.Services.Interfaces;
using Vision.Shared;
using Vision.Shared.Interfaces;

namespace Vision.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddVisionServices(this IServiceCollection services)
        {
            SharedServicesStartup.AddSharedServices(services);

            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IStorageService, StorageService>();

            services.AddScoped<IDataProviderStartup, VisionDataProviderStartup>();
            services.AddScoped<IDataProviderStartup, IdentityDataProviderStartup>();

            var serviceProvider = services.BuildServiceProvider();
            var providers = serviceProvider.GetServices<IDataProviderStartup>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            foreach (var provider in providers)
            {
                provider.AddDataProviders(services, configuration);
            }
            return services;
        }
    }
}
