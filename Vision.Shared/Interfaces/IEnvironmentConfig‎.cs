namespace Vision.Shared.Interfaces
{
    public interface IEnvironmentConfig‎
    {
        public string StorageConnectionString { get; }
        public string AzureAdBaseUri { get; }
        public string AzureAdAuthorizeEndpoint { get; }
        public string AzureAdTokenEndpoint { get; }
        public string AzureAdClientId { get; }
        public string AzureAdClientSecret { get; }
        public string AzureAdScope { get; }
        public string AzureAdRedirectUri { get; }
        public string AzureStorageTableName { get; }
        public string AzureStoragePartitionKey { get; }

    }
}
