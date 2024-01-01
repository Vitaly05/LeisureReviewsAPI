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

        protected IActionResult InvalidModelState() => BadRequest(new ErrorResponseModel { Code = 0, Message = "Invalid request model" });
    }
}
