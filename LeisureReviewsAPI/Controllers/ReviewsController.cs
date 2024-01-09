using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
using LeisureReviewsAPI.Models.ViewModels;
using LeisureReviewsAPI.Repositories;
using LeisureReviewsAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LeisureReviewsAPI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : BaseController
    {
        private readonly IReviewsRepository reviewsRepository;

        private readonly ILeisuresRepository leisuresRepository;
        
        private readonly ITagsRepository tagsRepository;

        public ReviewsController(
            IUsersRepository usersRepository, 
            IReviewsRepository reviewsRepository,
            ILeisuresRepository leisuresRepository,
            ITagsRepository tagsRepository
        ) : base(usersRepository)
        {
            this.reviewsRepository = reviewsRepository;
            this.leisuresRepository = leisuresRepository;
            this.tagsRepository = tagsRepository;
        }


        [HttpGet("get-page/{page}/{sortTarget}/{sortType}")]
        public async Task<List<ReviewDto>> GetReviewsPage(int page, string sortTarget, string sortType)
        {
            return await getReviewsDtoListAsync(getReviewSortModel(sortTarget, sortType), r => true, page);
        }

        [HttpGet("get-user-page/{username}/{page}/{sortTarget}/{sortType}")]
        public async Task<IActionResult> GetReviewsPage(string username, int page, string sortTarget, string sortType)
        {
            var user = await usersRepository.FindAsync(username);
            if (user is null) return NotFound();
            return Ok(await getReviewsDtoListAsync(getReviewSortModel(sortTarget, sortType), r => r.AuthorId == user.Id, page));
        }

        [HttpGet("get-pages-count")]
        public async Task<int> GetPagesCount() => await reviewsRepository.GetPagesCountAsync(5, r => true);

        [HttpGet("get-user-pages-count")]
        public async Task<IActionResult> GetUserPagesCount(string username)
        {
            var user = await usersRepository.FindAsync(username);
            if (user is null) return NotFound();
            return Ok(await reviewsRepository.GetPagesCountAsync(5, r => r.AuthorId == user.Id));
        }

        [HttpGet("get-review/{reviewId}")]
        public async Task<IActionResult> GetReview(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            return Ok(new ReviewDto(review, true));
        }

        [Authorize]
        [HttpPost("save-review")]
        public async Task<IActionResult> SaveReview(ReviewModel reviewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (!await canSaveAndEditAsync(reviewModel.AuthorId)) return Forbid();
            await saveReviewAsync(reviewModel);
            return Ok();
        }


        private ReviewSortModel getReviewSortModel(string sortTarget, string sortType)
        {
            Enum.TryParse(sortTarget, out ReviewSortTarget target);
            Enum.TryParse(sortType, out SortType type);
            return new ReviewSortModel { Target = target, Type = type };
        }

        private async Task<List<ReviewDto>> getReviewsDtoListAsync(ReviewSortModel reviewSortModel, Expression<Func<Review, bool>> predicate, int page)
        {
            var reviews = await getReviewsAsync(reviewSortModel, predicate, page, 5);
            return reviews.Select(r => new ReviewDto(r)).ToList();
        }

        private async Task<List<Review>> getReviewsAsync(ReviewSortModel sortModel, Expression<Func<Review, bool>> predicate, int page, int pageSize) =>
            sortModel.Target switch
            {
                ReviewSortTarget.Date => await reviewsRepository.GetLatestAsync(predicate, sortModel.Type, page, pageSize),
                ReviewSortTarget.Rate => await reviewsRepository.GetTopRatedAsync(predicate, sortModel.Type, page, pageSize),
                ReviewSortTarget.Likes => await reviewsRepository.GetTopLikedAsync(predicate, sortModel.Type, page, pageSize),
                _ => new()
            };

        private async Task saveReviewAsync(ReviewModel reviewModel)
        {
            var tags = await addTagsAsync(reviewModel.TagsNames);
            var leisure = await leisuresRepository.AddAsync(reviewModel.LeisureName);
            await reviewsRepository.SaveAsync(reviewModel.ConvertToReview(tags, leisure));
            //await updateIllustrationAsync(reviewModel);
        }

        private async Task<ICollection<Tag>> addTagsAsync(List<string> tagsNames)
        {
            if (tagsNames is null) return new List<Tag>();
            await tagsRepository.AddNewAsync(tagsNames);
            return await tagsRepository.GetAsync(tagsNames);
        }
    }
}
