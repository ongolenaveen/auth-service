namespace Vision.Data.Models
{
    public class UserToken
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public string? IdToken { get; set; }

        public DateTime? IssuedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

    }
}
