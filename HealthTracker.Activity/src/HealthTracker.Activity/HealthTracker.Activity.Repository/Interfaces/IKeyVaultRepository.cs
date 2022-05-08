using Azure.Security.KeyVault.Secrets;

namespace HealthTracker.Activity.Repository.Interfaces
{
    public interface IKeyVaultRepository
    {
        Task<KeyVaultSecret> RetrieveSecretFromKeyVault(string secretName);
    }
}
