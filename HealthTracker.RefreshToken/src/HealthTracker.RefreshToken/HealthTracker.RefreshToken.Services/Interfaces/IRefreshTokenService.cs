using HealthTracker.RefreshToken.Common.Models;

namespace HealthTracker.RefreshToken.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenResponse> RefreshToken();
    }
}
