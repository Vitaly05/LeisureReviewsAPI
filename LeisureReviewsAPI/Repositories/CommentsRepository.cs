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

        public async Task<List<Comment>> GetCommentsAsync(string reviewId) =>
            (await context.Reviews
                .Include(r => r.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == reviewId)).Comments.OrderByDescending(c => c.CreateTime).ToList();
    }
}
