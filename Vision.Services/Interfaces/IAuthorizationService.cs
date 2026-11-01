using Vision.Data.Models;

namespace Vision.Services.Interfaces
{
    public interface IAuthorizationService
    {
        Task<(UserToken Token, string SessionId)> ExchangeCodeForTokenAsync(AuthCodeRequest? authCodeRequest);
    }
}
