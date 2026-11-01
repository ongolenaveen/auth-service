using Vision.Data.Models;

namespace Vision.Services.Interfaces
{
    public interface IStorageService
    {
        Task CreateTokenAsync(UserToken? token);

        Task<UserToken?> ReadTokenAsync(string sessionId);

        Task UpdateTokenAsync(string sessionId, UserToken? token);

        Task DeleteTokenAsync(string sessionId);

    }
}
