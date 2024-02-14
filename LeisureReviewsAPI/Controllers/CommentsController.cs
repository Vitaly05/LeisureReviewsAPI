using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : BaseController
    {
        private readonly ICommentsRepository commentsRepository;

        public CommentsController(IUsersRepository usersRepository, ICommentsRepository commentsRepository) 
            : base(usersRepository)
        {
            this.commentsRepository = commentsRepository;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-current-user-rate")]
        public async Task<IActionResult> GetCurrentUserRate(string commentId) =>
            Ok(await commentsRepository.GetUserRate(User.FindFirstValue(ClaimTypes.NameIdentifier), commentId));
    }
}
