using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface ICommentsRepository
    {
        Task SaveAsync(Comment comment);

        Task<List<Comment>> GetCommentsAsync(string reviewId);
    }
}
