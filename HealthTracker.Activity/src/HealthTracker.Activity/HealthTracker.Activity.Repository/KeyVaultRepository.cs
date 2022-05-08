using Azure.Security.KeyVault.Secrets;
using HealthTracker.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Activity.Repository
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
    }
}
