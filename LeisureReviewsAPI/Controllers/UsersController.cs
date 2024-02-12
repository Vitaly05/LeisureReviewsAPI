using LeisureReviewsAPI.Attributes;
using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly ILikesRepository likesRepository;

        private readonly UserManager<User> userManager;

        public UsersController(IUsersRepository usersRepository, ILikesRepository likesRepository, UserManager<User> userManager) : base(usersRepository)
        {
            this.likesRepository = likesRepository;
            this.userManager = userManager;
        }


        [HttpGet("get-info")]
        public async Task<IActionResult> GetUserInfo(string username)
        {
            var user = await usersRepository.FindAsync(username);
            return await getUserInfoAsync(user);
        }

        [HttpGet("get-info-by-id/{userId}")]
        public async Task<IActionResult> GetUserInfoById(string userId)
        {
            var user = await usersRepository.GetByIdAsync(userId);
            return await getUserInfoAsync(user);
        }

        [HttpGet("get-username/{userId}")]
        public async Task<string> GetUserName(string userId) =>
            await usersRepository.GetUserNameAsync(userId);

        [AdminAuthorize()]
        [HttpGet("get-page/{page}")]
        public async Task<IActionResult> GetAllUsers(int page)
        {
            var users = await usersRepository.GetAllAsync(page, 10);
            foreach (var user in users)
            {
                user.LikesCount = await likesRepository.GetCountAsync(user);
                user.Roles = await usersRepository.GetRolesAsync(user);
                user.Likes.Clear();
                user.AuthoredReviews.Clear();
            }
            return Ok(users);
        }

        [AdminAuthorize()]
        [HttpGet("get-pages-count")]
        public async Task<IActionResult> GetPagesCount() =>
            Ok(await usersRepository.GetPagesCountAsync(10));

        [AdminAuthorize]
        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var user = await usersRepository.FindAsync(model.UserName);
            if (user is null) return BadRequest();
            await usersRepository.ChangeStatusAsync(user, model.Status);
            return Ok(model.Status);
        }

        [AdminAuthorize()]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var user = await usersRepository.FindAsync(model.UserName);
            if (user is null) return BadRequest();
            await userManager.AddToRoleAsync(user, model.Role.ToString());
            return Ok(model.Role);
        }


        private async Task<IActionResult> getUserInfoAsync(User user)
        {
            if (user is null) return NotFound();
            user.LikesCount = await likesRepository.GetCountAsync(user);
            return Ok(new UserDto(user));
        }
    }
}
