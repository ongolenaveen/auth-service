using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Data.Interfaces;
using Vision.Shared.Interfaces;

namespace Vision.DataProvider
{
    public class VisionDataProviderStartup : IDataProviderStartup
    {
        public void AddDataProviders(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var environmentConfig = serviceProvider.GetRequiredService<IEnvironmentConfig>();
            services.AddScoped<IVisionDataProvider, VisionDataProvider>();
            const string tableName = "usertokens";

            services.AddSingleton((_) =>
            {
                var connectionString = environmentConfig.StorageConnectionString;

                var tableClient = new TableClient( connectionString, tableName);

                tableClient.CreateIfNotExists();

                return tableClient;
            });
        }
    }
}
