using Google.Apis.Auth;
using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LeisureReviewsAPI.Repositories
{
    public class UsersRepository : BaseRepository, IUsersRepository
    {
        private readonly UserManager<User> userManager;

        public UsersRepository(UserManager<User> userManager, ApplicationContext context) : base(context)
        {
            this.userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(User newUser, string password)
        {
            if (await isUserExistsAsync(newUser.UserName))
                return IdentityResult.Failed(new IdentityError { Code = "3", Description = "Username is already taken" });
            return await userManager.CreateAsync(newUser, password);
        }

        public async Task<IdentityResult> CreateAsync(User newUser)
        {
            if (await isUserExistsAsync(newUser.UserName))
                return IdentityResult.Failed();
            return await userManager.CreateAsync(newUser);
        }

        public async Task<User> FindAsync(string userName) =>
            await userManager.FindByNameAsync(userName);

        public async Task<User> FindByGooglePayloadAsync(GoogleJsonWebSignature.Payload payload) =>
            await userManager.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

        public async Task<User> CreateByGoogleAsync(string userName, GoogleJsonWebSignature.Payload payload)
        {
            if (await isUserExistsAsync(userName))
                return null;

            var newUser = new User
            {
                UserName = userName,
                Email = payload.Email,
            };
            await userManager.CreateAsync(newUser);
            return newUser;
        }

        public async Task<User> GetAsync(ClaimsPrincipal principal) =>
            await userManager.GetUserAsync(principal);

        public async Task<User> GetByIdAsync(string id) => await userManager.FindByIdAsync(id);

        public async Task<string> GetUserName(string id) => 
            await context.Users.Where(u => u.Id == id).Select(u => u.UserName).FirstOrDefaultAsync();


        public async Task<User> GetWithoutQueryFiltersAsync(ClaimsPrincipal principal)
        {
            if (principal.Claims.IsNullOrEmpty()) return null;
            var id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllAsync(int page, int pageSize) =>
            await userManager.Users.AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        public async Task<int> GetPagesCountAsync(int pageSize) =>
            (int)Math.Ceiling(await userManager.Users.CountAsync() / (double)pageSize);

        public async Task<List<string>> GetRolesAsync(User user) =>
            await userManager.GetRolesAsync(user) as List<string>;

        public async Task<string> GetUserNameAsync(string id) =>
            (await userManager.FindByIdAsync(id))?.UserName;

        public async Task ChangeStatusAsync(User user, AccountStatus status)
        {
            user.Status = status;
            await userManager.UpdateAsync(user);
        }

        
        private async Task<bool> isUserExistsAsync(string userName)
        {
            var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserName == userName);
            return user is not null;
        }
    }
}
