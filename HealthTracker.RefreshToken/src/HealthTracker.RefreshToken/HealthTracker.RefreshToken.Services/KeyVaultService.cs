using HealthTracker.RefreshToken.Common.Models;
using HealthTracker.RefreshToken.Common.Settings;
using HealthTracker.RefreshToken.Repository.Interfaces;
using HealthTracker.RefreshToken.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthTracker.RefreshToken.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly Settings _settings;
        private readonly ILogger<KeyVaultService> _logger;

        public KeyVaultService(IKeyVaultRepository keyVaultRepository, IOptions<Settings> options, ILogger<KeyVaultService> logger)
        {
            _keyVaultRepository = keyVaultRepository;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SaveTokensToKeyVault(RefreshTokenResponse refreshTokenResponse)
        {
            try
            {
                _logger.LogInformation($"Saving {_settings.RefreshTokenName} and {_settings.FitbitCredentials} to Key Vault");
                await _keyVaultRepository.SaveSecretToKeyVault(_settings.RefreshTokenName, refreshTokenResponse.RefreshToken);
                await _keyVaultRepository.SaveSecretToKeyVault(_settings.FitbitCredentials, refreshTokenResponse.AccessToken);
                _logger.LogInformation("Tokens successfully saved to Key Vault.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SaveTokensToKeyVault)}: {ex.Message}");
                throw;
            }
        }
    }
}
