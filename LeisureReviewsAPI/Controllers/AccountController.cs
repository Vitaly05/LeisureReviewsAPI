using Google.Apis.Auth;
using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Repositories.Interfaces;
using LeisureReviewsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly IJwtHelper jwtHelper;

        private readonly UserManager<User> userManager;

        public AccountController(IUsersRepository usersRepository, IJwtHelper jwtHelper,
            UserManager<User> userManager) : base(usersRepository)
        {
            this.jwtHelper = jwtHelper;
            this.userManager = userManager;
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-account-info")]
        public IActionResult GetAccountInfo()
        {
            return Ok(new AccountInfoDto
            {
                Id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = HttpContext.User.FindFirstValue(ClaimTypes.Name)
            });
        }

        [HttpGet("check-create-review-access/{authorId}")]
        public async Task<IActionResult> CheckAccessToCreateReview(string authorId)
        {
            var currentUserInfo = await getCurrentUserInfoAsync();
            if (!currentUserInfo.IsAuthorized) return Forbid();
            if (await canSaveAndEditAsync(authorId)) return Ok();
            return Forbid();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(LoginModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelState();

            var user = await usersRepository.FindAsync(model.Username);
            if (user is null || !await passwordIsValidAsync(user, model.Password))
                return BadRequest(new ErrorResponseModel { Code = 1, Message = "Incorrect username or password" });
            if (!accountIsActive(user)) 
                return BadRequest(new ErrorResponseModel { Code = 2, Message = "Account has been blocked" });

            var response = await getAuthenticatedResponseAsync(user);
            addRefreshToken(user, response.RefreshToken);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return StatusCode(500);

            return Ok(response);
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(LoginModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelState();

            var user = new User { UserName = model.Username };
            var response = await getAuthenticatedResponseAsync(user);
            addRefreshToken(user, response.RefreshToken);

            var result = await usersRepository.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new ErrorResponseModel { Code = 3, Message = "Username is already taken" });

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] AuthModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var principal = jwtHelper.GetPrincipal(model.AccessToken);
            var user = await getCurrentUserAsync(principal.Identity.Name);
            if (user is null || !validateRefreshToken(user, model.RefreshToken)) return BadRequest("Invalid token");

            return await getTokenResponseAsync(user);
        }

        [HttpPost("google-sign-in")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleOAuthRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.TokenId, new GoogleJsonWebSignature.ValidationSettings());
            if (payload is null) return BadRequest();

            var user = await usersRepository.FindByGooglePayloadAsync(payload);
            if (user is null)
                return BadRequest(new ErrorResponseModel { Code = 5, Message = "Redirect to additional info" });

            return await getTokenResponseAsync(user);
        }

        [HttpPost("google-sign-up")]
        public async Task<IActionResult> GoogleSignUp(string userName, [FromBody] GoogleOAuthRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.TokenId, new GoogleJsonWebSignature.ValidationSettings());
            if (payload is null) return BadRequest();

            if (await usersRepository.FindByGooglePayloadAsync(payload) is not null) 
                return BadRequest();

            var user = await usersRepository.CreateByGoogleAsync(userName, payload);
            if (user is null)
                return BadRequest(new ErrorResponseModel { Code = 3, Message = "Username is already taken" });

            return await getTokenResponseAsync(user);
        }


        private async Task<AccountInfo> getCurrentUserInfoAsync()
        {
            var isAuthorized = HttpContext.User.Identity.IsAuthenticated;
            var model = new AccountInfo { IsAuthorized = isAuthorized };
            if (isAuthorized)
                model.CurrentUser = await usersRepository.GetAsync(HttpContext.User);
            return model;
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

        private async Task<IActionResult> getTokenResponseAsync(User user)
        {
            var response = await getAuthenticatedResponseAsync(user);
            addRefreshToken(user, response.RefreshToken);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return StatusCode(500);

            return Ok(response);
        }

        private async Task<AuthModel> getAuthenticatedResponseAsync(User user) =>
            new AuthModel
            {
                AccessToken = await jwtHelper.GenerateTokenAsync(user),
                RefreshToken = jwtHelper.GenerateRefreshToken()
            };

        private void addRefreshToken(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        }

        private bool validateRefreshToken(User user, string refreshToken) =>
            user.RefreshToken == refreshToken && user.RefreshTokenExpiryTime > DateTime.UtcNow;

        private async Task<User> getCurrentUserAsync(string userName) =>
            await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

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
