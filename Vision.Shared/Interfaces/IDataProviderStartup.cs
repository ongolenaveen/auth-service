using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vision.Shared.Interfaces
{
    public interface IDataProviderStartup
    {
        void AddDataProviders(IServiceCollection services, IConfiguration configuration);
    }
}
