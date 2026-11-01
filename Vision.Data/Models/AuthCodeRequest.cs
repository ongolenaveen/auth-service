namespace Vision.Data.Models
{
    public class AuthCodeRequest
    {
        public string Code { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;
    }
}
