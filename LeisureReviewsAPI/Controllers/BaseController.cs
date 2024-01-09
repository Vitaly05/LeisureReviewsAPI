using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviewsAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IUsersRepository usersRepository { get; init; }

        public BaseController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        protected async Task<bool> canSaveAndEditAsync(string authorId)
        {
            var user = await usersRepository.GetAsync(HttpContext.User);
            var isAdmin = (await usersRepository.GetRolesAsync(user)).Contains("Admin");
            return isAdmin || authorId == user.Id;
        }

        protected IActionResult InvalidModelState() => BadRequest(new ErrorResponseModel { Code = 0, Message = "Invalid request model" });
    }
}
