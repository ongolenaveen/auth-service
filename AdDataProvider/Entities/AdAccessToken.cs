using Newtonsoft.Json;

namespace AdDataProvider.Entities
{
    public class AdAccessToken
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; } = string.Empty;

        [JsonProperty("id_token")]
        public string? IdToken { get; set; } = string.Empty;

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonProperty("ext_expires_in")]
        public int? ExtExpiresIn { get; set; }

        public DateTime? ExpiresAt { get; set; }

    }
}
