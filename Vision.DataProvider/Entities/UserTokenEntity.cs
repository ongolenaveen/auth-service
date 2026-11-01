using Azure;
using Azure.Data.Tables;

namespace Vision.DataProvider.Entities
{
    public class UserTokenEntity : ITableEntity
    {
        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public string? IdToken { get; set; }

        public DateTime? IssuedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public ETag ETag { get; set; } = ETag.All;

        public DateTimeOffset? Timestamp { get; set; }

    }
}
