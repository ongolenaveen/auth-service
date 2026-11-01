using System.Net.Http.Headers;
using System.Text;
using AdDataProvider.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using Vision.Shared.Interfaces;

namespace AdDataProvider
{
    public class IdentityDataProvider(IHttpClientFactory httpClientFactory, IEnvironmentConfig environmentConfig, ILogger<IdentityDataProvider> logger) : IIdentityDataProvider
    {
        public async Task<UserToken> GetIdentityDataAsync(AuthCodeRequest authCodeRequest)
        {
            var grantType = "authorization_code";
            var clientId = environmentConfig.AzureAdClientId;
            var clientSecret = environmentConfig.AzureAdClientSecret;

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                throw new InvalidDataException("clientId or clientSecret environment variables are missing.");

            var form = new Dictionary<string, string>
            {
                {"grant_type", grantType},
                {"code", authCodeRequest.Code},
                {"state", authCodeRequest.State},
                {"scope", environmentConfig.AzureAdScope},
                {"redirect_uri", environmentConfig.AzureAdRedirectUri}
            };

            var httpClient = httpClientFactory.CreateClient("AzureAD");

            // Use basic auth, the token has already been converted to base64
            var basicBytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var basicB64 = Convert.ToBase64String(basicBytes);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicB64);

            var combinedTokenEndpointPath = $"{httpClient.BaseAddress}{environmentConfig.AzureAdTokenEndpoint}";
            logger.LogInformation("About to call token generation endpoint {CombinedTokenEndpointPath}.", combinedTokenEndpointPath);

            var tokenResponse = await httpClient.PostAsync(combinedTokenEndpointPath, new FormUrlEncodedContent(form));

            if (!tokenResponse.IsSuccessStatusCode)
                throw new InvalidOperationException($"{tokenResponse.StatusCode} {tokenResponse.ReasonPhrase}:Error while calling token endpoint {combinedTokenEndpointPath}.");

            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            var adAccessToken = JsonConvert.DeserializeObject<AdAccessToken>(jsonContent);
            if (adAccessToken == null || string.IsNullOrWhiteSpace(adAccessToken.AccessToken))
                throw new InvalidOperationException($"Error while deserializing response from the endpoint {combinedTokenEndpointPath}.");

            var expiresAt = DateTime.UtcNow.AddSeconds(adAccessToken.ExpiresIn ?? 3600);
            adAccessToken.ExpiresAt = expiresAt;

            var userToken = new UserToken
            {
                AccessToken = adAccessToken.AccessToken,
                RefreshToken = adAccessToken.RefreshToken,
                IdToken = adAccessToken.IdToken,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            return userToken;
        }
    }
}
