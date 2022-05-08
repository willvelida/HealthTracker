using HealthTracker.Activity.Common;
using HealthTracker.Activity.Repository.Interfaces;
using HealthTracker.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HealthTracker.Activity.Services
{
    public class FitbitService : IFitbitService
    {
        private readonly Settings _settings;
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FitbitService> _logger;

        public FitbitService(
            IOptions<Settings> options,
            IKeyVaultRepository keyVaultRepository,
            HttpClient httpClient,
            ILogger<FitbitService> logger)
        {
            _settings = options.Value;
            _keyVaultRepository = keyVaultRepository;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ActivityResponse> GetActivityResponse(string date)
        {
            try
            {
                var fitbitAccessToken = await _keyVaultRepository.RetrieveSecretFromKeyVault(_settings.AccessTokenName);
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getDailyActivityLogUri = new Uri($"https://api.fitbit.com/1/user/-/activities/date/{date}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getDailyActivityLogUri);
                request.Content = new StringContent("");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var activityResponse = JsonConvert.DeserializeObject<ActivityResponse>(responseString);

                return activityResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityResponse)}: {ex.Message}");
                throw;
            }
        }
    }
}
