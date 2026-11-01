using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using Vision.DataProvider.Entities;

namespace Vision.DataProvider
{
    public class VisionDataProvider(TableClient tableClient, ILogger<VisionDataProvider> logger) : IVisionDataProvider
    {
        /// <summary>
        /// Creates a new token entity.
        /// </summary>
        /// <param name="partitionKey">The partition key for the token.</param>
        /// <param name="rowKey"></param>
        /// <param name="token">The token to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateToken(string partitionKey, string rowKey, UserToken token)
        {
            var entity = new UserTokenEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                IdToken = token.IdToken,
                IssuedAt = token.IssuedAt,
                ExpiresAt = token.ExpiresAt
            };
            await tableClient.AddEntityAsync(entity);
            logger.LogInformation("Entity inserted successfully.");
        }

        /// <summary>
        /// Deletes a token entity based on the provided partition key and session ID.
        /// </summary>
        /// <param name="partitionKey">The partition key for the token.</param>
        /// <param name="sessionId">The session ID for the token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteToken(string partitionKey, string sessionId)
        {
            await tableClient.DeleteEntityAsync(partitionKey, sessionId);
            logger.LogInformation("Entity deleted successfully.");
        }

        /// <summary>
        /// Reads a token entity based on the provided partition key and session ID.
        /// </summary>
        /// <param name="partitionKey">The partition key for the token.</param>
        /// <param name="sessionId">The session ID for the token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<UserToken?> ReadToken(string partitionKey, string sessionId)
        {
            var entity = await tableClient.GetEntityAsync<UserTokenEntity>(partitionKey, sessionId);
            if (entity?.Value == null)
                return null;
            
            return new UserToken
            {
                AccessToken = entity.Value.AccessToken,
                RefreshToken = entity.Value.RefreshToken,
                IdToken = entity.Value.IdToken,
                IssuedAt = entity.Value.IssuedAt,
                ExpiresAt = entity.Value.ExpiresAt
            };
        }

        /// <summary>
        /// Updates Token entity based on the provided partition key and session ID.
        /// </summary>
        /// <param name="partitionKey">The partition key for the token.</param>
        /// <param name="sessionId">The session ID for the token.</param>
        /// <param name="token">The updated token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task UpdateToken(string partitionKey, string sessionId, UserToken token)
        {
            var entity = await tableClient.GetEntityAsync<UserTokenEntity>(partitionKey, sessionId);
            if (entity?.Value == null)
                throw new InvalidOperationException("Entity not found");
            
            entity.Value.IdToken = token.IdToken;
            entity.Value.AccessToken = token.AccessToken;
            entity.Value.RefreshToken = token.RefreshToken;
            entity.Value.IssuedAt = token.IssuedAt;
            entity.Value.ExpiresAt = token.ExpiresAt;

            await tableClient.UpsertEntityAsync<UserTokenEntity>(entity);

            logger.LogInformation("Entity updated successfully.");
        }
    }
}
