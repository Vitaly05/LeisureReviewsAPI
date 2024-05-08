using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Models.Dto;
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

        private readonly ILikesRepository likesRepository;

        public ReviewsController(
            IUsersRepository usersRepository, 
            IReviewsRepository reviewsRepository,
            ILeisuresRepository leisuresRepository,
            ITagsRepository tagsRepository,
            ILikesRepository likesRepository,
            IRatesRepository ratesRepository
        ) : base(usersRepository)
        {
            this.reviewsRepository = reviewsRepository;
            this.leisuresRepository = leisuresRepository;
            this.tagsRepository = tagsRepository;
            this.likesRepository = likesRepository;
        }


        [HttpGet("get-page/{page}/{sortTarget}/{sortType}")]
        public async Task<List<ReviewDto>> GetReviewsPage(int page, string sortTarget, string sortType, [FromQuery] string leisureGroup)
        {
            return await getReviewsDtoListAsync(getReviewSortModel(sortTarget, sortType, leisureGroup), r => true, page);
        }

        [HttpGet("get-user-page/{username}/{page}/{sortTarget}/{sortType}")]
        public async Task<IActionResult> GetReviewsPage(string username, int page, string sortTarget, string sortType, string leisureGroup = null)
        {
            var user = await usersRepository.FindAsync(username);
            if (user is null) return NotFound();
            return Ok(await getReviewsDtoListAsync(getReviewSortModel(sortTarget, sortType, leisureGroup), r => r.AuthorId == user.Id, page));
        }

        [HttpGet("get-tags-page/{page}/{sortTarget}/{sortType}")]
        public async Task<List<ReviewDto>> GetReviewsPageByTags(int page, string sortTarget, string sortType, [FromQuery] string leisureGroup, [FromQuery] List<string> tags)
        {
            if (tags.Count == 0)
                return await GetReviewsPage(page, sortTarget, sortType, leisureGroup);
            var f = await getReviewsDtoListAsync(
                getReviewSortModel(sortTarget, sortType, leisureGroup),
                r => r.Tags.Select(t => t.Name).Any(t => tags.Contains(t)),
                page);
            return f;
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

        [HttpGet("get-tags-pages-count")]
        public async Task<int> GetTagsPagesCount([FromQuery] List<string> tags, [FromQuery] string leisureGroup)
        {
            Enum.TryParse(leisureGroup, out LeisureGroup group);
            if (group == 0)
            {
                if (tags.Count == 0)
                    return await GetPagesCount();
                else
                    return await reviewsRepository.GetPagesCountAsync(5, r => r.Tags.Select(t => t.Name).Any(t => tags.Contains(t)));
            }
            else
            {
                if (tags.Count == 0)
                    return await reviewsRepository.GetPagesCountAsync(5, r => r.Group == group);
                else
                    return await reviewsRepository.GetPagesCountAsync(5, r => r.Group == group && r.Tags.Select(t => t.Name).Any(t => tags.Contains(t)));
            }            
        }

        [HttpGet("get-review/{reviewId}")]
        public async Task<IActionResult> GetReview(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            return Ok(new ReviewDto(review, true));
        }

        [HttpGet("get-related-reviews/{reviewId}")]
        public async Task<IActionResult> GetRelatedReviews(string reviewId)
        {
            var relatedReviews = await reviewsRepository.GetRelatedAsync(reviewId, 5);
            var relatedReviewsDto = relatedReviews.Select(r => new ReviewDto(r));
            return Ok(relatedReviewsDto);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("save-review")]
        public async Task<IActionResult> SaveReview(ReviewModel reviewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (!await canSaveAndEditAsync(reviewModel.AuthorId)) return Forbid();
            var reviewId = await saveReviewAsync(reviewModel);
            return Ok(reviewId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("delete-review/{reviewId}")]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            if (!await canSaveAndEditAsync(review.AuthorId)) return Forbid();
            await reviewsRepository.DeleteAsync(reviewId);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("can-like/{reviewId}")]
        public async Task<IActionResult> CanLike(string reviewId)
        {
            var user = await usersRepository.GetAsync(HttpContext.User);
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            if (await likesRepository.HasLikeAsync(user, review))
                return Ok(false);
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("like-review/{reviewId}")]
        public async Task<IActionResult> LikeReview(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            await likesRepository.LikeAsync(review, await usersRepository.GetAsync(HttpContext.User));
            return Ok();
        }


        private ReviewSortModel getReviewSortModel(string sortTarget, string sortType, string leisureGroup)
        {
            Enum.TryParse(sortTarget, out ReviewSortTarget target);
            Enum.TryParse(sortType, out SortType type);
            Enum.TryParse(leisureGroup, out LeisureGroup group);
            return new ReviewSortModel { Target = target, Type = type, LeisureGroup = group == 0 ? null : group };
        }

        private async Task<List<ReviewDto>> getReviewsDtoListAsync(ReviewSortModel reviewSortModel, Expression<Func<Review, bool>> predicate, int page)
        {
            if (page < 1) page = 1;
            var reviews = await getReviewsAsync(reviewSortModel, predicate, page, 5);
            return reviews.Select(r => new ReviewDto(r)).ToList();
        }

        private async Task<List<Review>> getReviewsAsync(ReviewSortModel sortModel, Expression<Func<Review, bool>> predicate, int page, int pageSize) =>
            sortModel.Target switch
            {
                ReviewSortTarget.Date => await reviewsRepository.GetLatestAsync(predicate, sortModel.Type, sortModel.LeisureGroup, page, pageSize),
                ReviewSortTarget.Rate => await reviewsRepository.GetTopRatedAsync(predicate, sortModel.Type, sortModel.LeisureGroup, page, pageSize),
                ReviewSortTarget.Likes => await reviewsRepository.GetTopLikedAsync(predicate, sortModel.Type, sortModel.LeisureGroup, page, pageSize),
                _ => new()
            };

        private async Task<string> saveReviewAsync(ReviewModel reviewModel)
        {
            var tags = await addTagsAsync(reviewModel.TagsNames);
            var leisure = await leisuresRepository.AddAsync(reviewModel.LeisureName);
            var reviewId = await reviewsRepository.SaveAsync(reviewModel.ConvertToReview(tags, leisure));
            return reviewId;
        }

        private async Task<ICollection<Tag>> addTagsAsync(List<string> tagsNames)
        {
            if (tagsNames is null) return new List<Tag>();
            await tagsRepository.AddNewAsync(tagsNames);
            return await tagsRepository.GetAsync(tagsNames);
        }
    }
}
