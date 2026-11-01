using Microsoft.Extensions.Configuration;
using Vision.Shared.Interfaces;

namespace Vision.Shared
{
    public class EnvironmentConfig(IConfiguration config): IEnvironmentConfig
    {
        public string StorageConnectionString => Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING") ?? string.Empty;
        public string AzureAdAuthorizeEndpoint => Environment.GetEnvironmentVariable("AZURE_AD_AUTHORIZE_ENDPOINT") ?? string.Empty;
        public string AzureAdTokenEndpoint => Environment.GetEnvironmentVariable("AZURE_AD_TOKEN_ENDPOINT") ?? string.Empty;
        public string AzureAdClientId => Environment.GetEnvironmentVariable("AZURE_AD_CLIENT_ID") ?? string.Empty;
        public string AzureAdClientSecret => Environment.GetEnvironmentVariable("AZURE_AD_CLIENT_SECRET") ?? string.Empty;
        public string AzureAdScope => Environment.GetEnvironmentVariable("AZURE_AD_SCOPE") ?? string.Empty;
        public string AzureAdRedirectUri => Environment.GetEnvironmentVariable("AZURE_AD_REDIRECT_URI") ?? string.Empty;
        public string AzureAdBaseUri => Environment.GetEnvironmentVariable("AZURE_AD_BASE_URI") ?? string.Empty;
        public string AzureStorageTableName => Environment.GetEnvironmentVariable("AZURE_STORAGE_TABLE_NAME") ?? string.Empty;
        public string AzureStoragePartitionKey => Environment.GetEnvironmentVariable("AZURE_STORAGE_PARTITION_KEY") ?? string.Empty;

    }
}
