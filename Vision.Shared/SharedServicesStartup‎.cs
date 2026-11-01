using Microsoft.Extensions.DependencyInjection;
using Vision.Shared.Interfaces;

namespace Vision.Shared
{
    public class SharedServicesStartup
    {
        public static void AddSharedServices(IServiceCollection services)
        {
            services.AddScoped<IEnvironmentConfig, EnvironmentConfig>();
        }
    }
}
