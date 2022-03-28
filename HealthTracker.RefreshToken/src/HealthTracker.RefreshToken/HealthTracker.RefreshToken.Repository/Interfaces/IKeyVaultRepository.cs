using Azure.Security.KeyVault.Secrets;

namespace HealthTracker.RefreshToken.Repository.Interfaces
{
    public interface IKeyVaultRepository
    {
        Task<KeyVaultSecret> RetrieveSecretFromKeyVault(string secretName);
        Task SaveSecretToKeyVault(string secretName, string secretValue);
    }
}
