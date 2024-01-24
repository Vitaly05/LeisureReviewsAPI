using LeisureReviewsAPI.Models.Database;
using System.Security.Claims;

namespace LeisureReviewsAPI.Services.Interfaces
{
    public interface IJwtHelper
    {
        Task<string> GenerateTokenAsync(User user);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipal(string token);
    }
}
