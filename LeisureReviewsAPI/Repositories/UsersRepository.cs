﻿using LeisureReviewsAPI.Data;
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

        public async Task<IdentityResult> CreateAsync(User user, string password) => 
            await userManager.CreateAsync(user, password);

        public async Task<IdentityResult> CreateAsync(User user) =>
            await userManager.CreateAsync(user);

        public async Task<User> FindAsync(string userName) =>
            await userManager.FindByNameAsync(userName);

        public async Task<User> FindAsync(string externalProvider, string providerKey) => 
            await userManager.Users.FirstOrDefaultAsync(u => u.ExternalProvider == externalProvider && u.ProviderKey == providerKey);

        public async Task<User> GetAsync(ClaimsPrincipal principal) =>
            await userManager.GetUserAsync(principal);

        public async Task<User> GetByIdAsync(string id) => await userManager.FindByIdAsync(id);

        public async Task<User> GetWithoutQueryFiltersAsync(ClaimsPrincipal principal)
        {
            if (principal.Claims.IsNullOrEmpty()) return null;
            var id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllAsync(int page, int pageSize) =>
            await userManager.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        public async Task<int> GetPagesCountAsync(int pageSize) =>
            (int)Math.Ceiling(await userManager.Users.CountAsync() / (double)pageSize);

        public async Task<List<string>> GetRolesAsync(User user) =>
            await userManager.GetRolesAsync(user) as List<string>;

        public async Task<string> GetUserNameAsync(string id) =>
            (await userManager.FindByIdAsync(id)).UserName;

        public async Task ChangeStatusAsync(User user, AccountStatus status)
        {
            user.Status = status;
            await userManager.UpdateAsync(user);
        }
    }
}
