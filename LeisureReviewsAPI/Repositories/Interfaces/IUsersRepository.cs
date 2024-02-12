using Google.Apis.Auth;
using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models.Database;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> CreateAsync(User user);

        Task<User> FindAsync(string userName);

        Task<User> FindByGooglePayloadAsync(GoogleJsonWebSignature.Payload payload);

        Task<User> CreateByGoogleAsync(string userName, GoogleJsonWebSignature.Payload payload);

        Task<User> GetAsync(ClaimsPrincipal principal);

        Task<User> GetByIdAsync(string id);

        Task<string> GetUserName(string id);

        Task<User> GetWithoutQueryFiltersAsync(ClaimsPrincipal principal);

        Task<List<User>> GetAllAsync(int page, int pageSize);

        Task<int> GetPagesCountAsync(int pageSize);

        Task<List<string>> GetRolesAsync(User user);

        Task<string> GetUserNameAsync(string id);

        Task ChangeStatusAsync(User user, AccountStatus status);
    }
}
