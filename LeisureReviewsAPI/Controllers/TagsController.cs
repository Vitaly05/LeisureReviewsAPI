using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("/api/tags")]
    public class TagsController : BaseController
    {
        private readonly ITagsRepository tagsRepository;
        public TagsController(IUsersRepository usersRepository, ITagsRepository tagsRepository) : base(usersRepository)
        {
            this.tagsRepository = tagsRepository;
        }

        [HttpGet("get-weights")]
        public async Task<IActionResult> GetTagsWeights() =>
            Ok(await tagsRepository.GetWeightsAsync());
    }
}
