using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Models.ViewModels;
using LeisureReviewsAPI.Repositories;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly ILikesRepository likesRepository;

        public UsersController(IUsersRepository usersRepository, ILikesRepository likesRepository) : base(usersRepository)
        {
            this.likesRepository = likesRepository;
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


        private async Task<IActionResult> getUserInfoAsync(User user)
        {
            if (user is null) return NotFound();
            user.LikesCount = await likesRepository.GetCountAsync(user);
            return Ok(new UserDto(user));
        }
    }
}
