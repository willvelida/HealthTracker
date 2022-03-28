using Azure.Security.KeyVault.Secrets;
using HealthTracker.RefreshToken.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace HealthTracker.RefreshToken.Repository
{
    public class KeyVaultRepository : IKeyVaultRepository
    {
        private readonly SecretClient _secretClient;
        private readonly ILogger<KeyVaultRepository> _logger;

        public KeyVaultRepository(SecretClient secretClient, ILogger<KeyVaultRepository> logger)
        {
            _secretClient = secretClient;
            _logger = logger;
        }
        public async Task<KeyVaultSecret> RetrieveSecretFromKeyVault(string secretName)
        {
            try
            {
                KeyVaultSecret keyVaultSecret = await _secretClient.GetSecretAsync(secretName);
                return keyVaultSecret;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(RetrieveSecretFromKeyVault)}: {ex.Message}");
                throw;
            }
        }

        public async Task SaveSecretToKeyVault(string secretName, string secretValue)
        {
            try
            {
                await _secretClient.SetSecretAsync(secretName, secretValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SaveSecretToKeyVault)}: {ex.Message}");
                throw;
            }
        }
    }
}
