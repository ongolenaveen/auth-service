using Vision.Data.Models;

namespace Vision.Data.Interfaces
{
    public interface IVisionDataProvider
    {
        Task CreateToken(string partitionKey, string rowKey, UserToken token);

        Task<UserToken?> ReadToken(string partitionKey, string sessionId);

        Task UpdateToken(string partitionKey, string sessionId, UserToken token);

        Task DeleteToken(string partitionKey, string sessionId);
    }
}
