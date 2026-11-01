using Microsoft.Extensions.Logging;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using Vision.Services.Interfaces;
using Vision.Shared.Interfaces;

namespace Vision.Services
{
    public class AuthorizationService(IIdentityDataProvider identityDataProvider, IVisionDataProvider visionDataProvider, IEnvironmentConfig environmentConfig, ILogger<AuthorizationService> logger) : IAuthorizationService
    {
        public async Task<(UserToken Token, string SessionId)> ExchangeCodeForTokenAsync(AuthCodeRequest? authCodeRequest)
        {
            if (authCodeRequest == null)
            {
                logger.LogWarning("Received null authorization code request.");
                throw new ArgumentNullException(nameof(authCodeRequest));
            }
            logger.LogInformation("Exchanging authorization code for token.");
            var partitionKey = environmentConfig.AzureStoragePartitionKey;

            var userToken = await identityDataProvider.GetIdentityDataAsync(authCodeRequest);

            var rowKey = Guid.NewGuid().ToString();

            await visionDataProvider.CreateToken(partitionKey, rowKey, userToken);

            return (userToken, rowKey);
        }
    }
}
