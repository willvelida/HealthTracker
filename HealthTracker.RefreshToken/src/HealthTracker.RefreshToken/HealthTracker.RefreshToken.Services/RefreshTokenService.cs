using HealthTracker.RefreshToken.Common.Models;
using HealthTracker.RefreshToken.Common.Settings;
using HealthTracker.RefreshToken.Repository.Interfaces;
using HealthTracker.RefreshToken.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HealthTracker.RefreshToken.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly Settings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RefreshTokenService> _logger;

        public RefreshTokenService(
            IKeyVaultRepository keyVaultRepository,
            IOptions<Settings> options,
            HttpClient httpClient,
            ILogger<RefreshTokenService> logger)
        {
            _keyVaultRepository = keyVaultRepository;
            _settings = options.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<RefreshTokenResponse> RefreshToken()
        {
            try
            {
                var fitbitRefreshTokenSecret = await _keyVaultRepository.RetrieveSecretFromKeyVault(_settings.RefreshTokenName);
                if (fitbitRefreshTokenSecret is null)
                    throw new NullReferenceException("The Fitbit refresh token could not be retrieved. Verify that the secret exists in Key Vault.");

                var fitbitClientCredentials = await _keyVaultRepository.RetrieveSecretFromKeyVault(_settings.FitbitCredentials);
                if (fitbitClientCredentials is null)
                    throw new NullReferenceException("The Fitbit client credentials could not be retrieved. Verify that the secret exists in Key Vault.");

                _httpClient.DefaultRequestHeaders.Clear();
                UriBuilder uri = new UriBuilder("https://api.fitbit.com/oauth2/token");
                uri.Query = $"grant_type=refresh_token&refresh_token={fitbitRefreshTokenSecret.Value}";
                var request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);
                request.Content = new StringContent("");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", fitbitClientCredentials.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var refreshTokenResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(content);

                return refreshTokenResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(RefreshTokenService)}: {ex.Message}");
                throw;
            }
        }
    }
}
