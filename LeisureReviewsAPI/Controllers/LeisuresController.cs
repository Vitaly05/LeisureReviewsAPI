using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Repositories.Interfaces;
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
    }
}
