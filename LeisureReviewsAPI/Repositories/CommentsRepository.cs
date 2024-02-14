using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Repositories.Interfaces;
using LeisureReviewsAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviewsAPI.Repositories
{
    public class CommentsRepository : BaseRepository, ICommentsRepository
    {
        private readonly ISearchService searchService;

        public CommentsRepository(ApplicationContext context, ISearchService searchService) : base(context)
        {
            this.searchService = searchService;
        }

        public async Task SaveAsync(Comment comment)
        {
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
            await searchService.UpdateReviewAsync(await context.Reviews.FirstOrDefaultAsync(r => r.Id == comment.Review.Id));
        }

        public async Task<Comment> GetByIdAsync(string id) =>
            await contextComments().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<Comment>> GetCommentsAsync(string reviewId) =>
            await contextComments()
                .Where(c => c.Review.Id == reviewId)
                .OrderByDescending(c => c.CreateTime)
                .ToListAsync();

        public async Task RateAsync(bool isPositive, string userId, string commentId)
        {
            bool newRate = false;
            var rate = await context.CommentRates
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);
            if (rate is null)
            {
                newRate = true;
                rate = new CommentRate
                {
                    IsPositive = isPositive,
                    UserId = userId,
                    CommentId = commentId
                };
            }
            if (newRate)
                await context.CommentRates.AddAsync(rate);
            else
            {
                rate.IsPositive = isPositive;
                context.CommentRates.Update(rate);
            }
            await context.SaveChangesAsync();
        }

        public async Task<bool?> GetUserRate(string userId, string commentId)
        {
            var comment = await context.Comments
                .Include(c => c.Rates)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment?.Rates?.Count == 0) return null;
            return comment.Rates.FirstOrDefault(r => r.UserId == userId)?.IsPositive;
        }


        private IQueryable<Comment> contextComments() =>
            context.Comments
                .AsNoTracking()
                .Include(c => c.Author)
                .Include(c => c.Rates)
                .Include(c => c.Review);
    }
}
