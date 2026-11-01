using Vision.Data.Models;

namespace Vision.Data.Interfaces
{
    public interface IIdentityDataProvider
    {
        Task<UserToken> GetIdentityDataAsync(AuthCodeRequest authCodeRequest);
    }
}
