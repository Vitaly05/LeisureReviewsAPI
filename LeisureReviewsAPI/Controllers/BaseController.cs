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
    }
}
