using HealthTracker.RefreshToken.Common.Models;

namespace HealthTracker.RefreshToken.Services.Interfaces
{
    public interface IKeyVaultService
    {
        Task SaveTokensToKeyVault(RefreshTokenResponse refreshTokenResponse);
    }
}
