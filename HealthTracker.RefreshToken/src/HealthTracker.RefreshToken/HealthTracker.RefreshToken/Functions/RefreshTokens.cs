using HealthTracker.RefreshToken.Common.Models;
using HealthTracker.RefreshToken.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HealthTracker.RefreshToken.Functions
{
    public class RefreshTokens
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IKeyVaultService _keyVaultService;
        private readonly ILogger<RefreshTokens> _logger;

        public RefreshTokens(IRefreshTokenService refreshTokenService, IKeyVaultService keyVaultService, ILogger<RefreshTokens> logger)
        {
            _refreshTokenService = refreshTokenService;
            _keyVaultService = keyVaultService;
            _logger = logger;
        }

        [FunctionName(nameof(RefreshTokens))]
        public async Task Run([TimerTrigger("0 0 */6 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"Attempting to refresh Fitbit refresh and access tokens: {DateTime.Now}");

                RefreshTokenResponse refreshTokenResponse = await _refreshTokenService.RefreshToken();

                _logger.LogInformation($"Refresh successful. Saving to Key Vault");
                await _keyVaultService.SaveTokensToKeyVault(refreshTokenResponse);
                _logger.LogInformation("Tokens saved.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(RefreshTokens)}: {ex.Message}");
                throw ex;
            }
        }
    }
}
