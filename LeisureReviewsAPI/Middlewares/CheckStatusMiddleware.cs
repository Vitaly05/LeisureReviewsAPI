using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LeisureReviewsAPI.Middlewares
{
    public class CheckStatusMiddleware
    {
        private readonly RequestDelegate next;

        public CheckStatusMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUsersRepository usersRepository, SignInManager<User> signInManager)
        {
            var user = await usersRepository.GetWithoutQueryFiltersAsync(context.User);
            if (!hasAccess(user))
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/");
            }
            await next.Invoke(context);
        }

        private bool hasAccess(User user)
        {
            if (user is null) return true;
            if (user.Status != Data.AccountStatus.Active)
                return false;
            return true;
        }
    }
}
