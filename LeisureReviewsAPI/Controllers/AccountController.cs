using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly SignInManager<User> signInManager;

        private readonly UserManager<User> userManager;

        public AccountController(IUsersRepository usersRepository, SignInManager<User> signInManager,
            UserManager<User> userManager) : base(usersRepository)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }


        [HttpGet("check-auth")]
        public async Task<AccountInfo> CheckAuth() => await getCurrentUserInfoAsync();

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponseModel { Code = 0, Message = "Invalid request model" });
            var user = await signInAsync(model);
            if (user is null)
                return BadRequest(new ErrorResponseModel { Code = 1, Message = "Incorrect username or password" });
            var accountInfo = new AccountInfo
            {
                CurrentUser = new UserDto(user),
                IsAuthorized = true
            };
            return Ok(accountInfo);
        }

        [HttpPost("sign-out")]
        public async Task<IActionResult> SignUserOut()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
        

        private async Task<AccountInfo> getCurrentUserInfoAsync()
        {
            var isAuthorized = HttpContext.User.Identity.IsAuthenticated;
            var model = new AccountInfo { IsAuthorized = isAuthorized };
            if (isAuthorized)
                model.CurrentUser = new UserDto(await usersRepository.GetAsync(HttpContext.User));
            return model;
        }

        private async Task<User> signInAsync(LoginModel model)
        {
            var user = await usersRepository.FindAsync(model.Username);
            if (!await passwordIsValidAsync(user, model.Password) || !accountIsActive(user)) return null;
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            return result.Succeeded ? user : null;
        }

        private async Task<bool> passwordIsValidAsync(User user, string password)
        {
            if (!await userManager.CheckPasswordAsync(user, password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                return false;
            }
            return true;
        }

        private bool accountIsActive(User user)
        {
            if (user.Status == AccountStatus.Blocked)
            {
                ModelState.AddModelError(string.Empty, "Account was blocked.");
                return false;
            }
            return true;
        }
    }
}
