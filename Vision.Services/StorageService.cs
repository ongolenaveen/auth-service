using Microsoft.Extensions.Logging;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using Vision.Services.Interfaces;
using Vision.Shared.Interfaces;

namespace Vision.Services
{
    public class StorageService (IVisionDataProvider visionDataProvider, IEnvironmentConfig environmentConfig, ILogger<StorageService> logger) : IStorageService
    {

        public async Task CreateTokenAsync(UserToken? token)
        {
            logger.LogInformation("Creating token for user.");
            if (token == null)
            {
                logger.LogWarning("Attempted to create a null token.");
                return;
            }

            var sessionId = Guid.NewGuid().ToString();
            await visionDataProvider.CreateToken(environmentConfig.AzureStoragePartitionKey, sessionId, token);
        }

        public async Task DeleteTokenAsync(string sessionId)
        {
            logger.LogInformation("Deleting token for user.");
            await visionDataProvider.DeleteToken(environmentConfig.AzureStoragePartitionKey, sessionId);
        }

        public async Task<UserToken?> ReadTokenAsync(string sessionId)
        {
            logger.LogInformation("Reading token for user.");
            return await visionDataProvider.ReadToken(environmentConfig.AzureStoragePartitionKey, sessionId);
        }

        public async Task UpdateTokenAsync(string sessionId, UserToken? token)
        {
            logger.LogInformation("Updating token for user.");
            if (token == null)
            {
                logger.LogWarning("Attempted to update with a null token.");
                return;
            }
            await visionDataProvider.UpdateToken("users", sessionId, token);
        }
    }
}
