using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/leisures")]
    public class LeisuresController : BaseController
    {
        private readonly ILeisuresRepository leisuresRepository;

        private readonly IRatesRepository ratesRepository;

        public LeisuresController(IUsersRepository usersRepository, ILeisuresRepository leisuresRepository, IRatesRepository ratesRepository) : base(usersRepository)
        {
            this.leisuresRepository = leisuresRepository;
            this.ratesRepository = ratesRepository;
        }

        [HttpGet("get-info/{leisureId}")]
        public async Task<LeisureDto> GetLeisureInfo(string leisureId)
        {
            var leisure = await leisuresRepository.GetAsync(leisureId);
            var averageRate = await ratesRepository.GetAverageRateAsync(leisure);
            return new LeisureDto(leisure, averageRate);
        }

        [Authorize]
        [HttpGet("get-rate/{leisureId}")]
        public async Task<IActionResult> GetRate(string leisureId)
        {
            var leisure = await leisuresRepository.GetAsync(leisureId);
            if (leisure is null) return NotFound();
            var rate = await ratesRepository.GetAsync(await usersRepository.GetAsync(HttpContext.User), leisure);
            return Ok(rate?.Value);
        }

        [Authorize]
        [HttpPost("rate/{leisureId}/{value}")]
        public async Task<IActionResult> RateReview(string leisureId, int value)
        {
            var leisure = await leisuresRepository.GetAsync(leisureId);
            if (leisure is null) return NotFound();
            var rate = new Rate
            {
                Leisure = leisure,
                User = await usersRepository.GetAsync(HttpContext.User),
                Value = value
            };
            await ratesRepository.SaveAsync(rate);
            return Ok(await ratesRepository.GetAverageRateAsync(rate.Leisure));
        }
    }
}
