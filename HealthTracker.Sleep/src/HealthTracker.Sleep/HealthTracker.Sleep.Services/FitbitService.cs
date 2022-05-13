using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using HealthTracker.Sleep.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HealthTracker.Sleep.Services
{
    public class FitbitService : IFitbitService
    {
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly Settings _settings;
        private HttpClient _httpClient;
        private readonly ILogger<FitbitService> _logger;

        public FitbitService(IKeyVaultRepository keyVaultRepository, HttpClient httpClient, ILogger<FitbitService> logger, IOptions<Settings> options)
        {
            _keyVaultRepository = keyVaultRepository;
            _httpClient = httpClient;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task<SleepResponse> GetSleepResonse(string date)
        {
            try
            {
                var fitbitAccessToken = await _keyVaultRepository.RetrieveSecretFromKeyVault(_settings.AccessTokenName);
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getDailySleepLogUri = new Uri($"https://api.fitbit.com/1/user/-/sleep/date/{date}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getDailySleepLogUri);
                request.Content = new StringContent("");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                var sleepResponse = JsonConvert.DeserializeObject<SleepResponse>(responseString);

                return sleepResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepResonse)}: {ex.Message}");
                throw;
            }
        }
    }
}
