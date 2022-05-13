using Azure.Security.KeyVault.Secrets;

namespace HealthTracker.Sleep.Repository.Interfaces
{
    public interface IKeyVaultRepository
    {
        Task<KeyVaultSecret> RetrieveSecretFromKeyVault(string secretName);
    }
}
