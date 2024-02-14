using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface ICommentsRepository
    {
        Task SaveAsync(Comment comment);

        Task<Comment> GetByIdAsync(string id);

        Task<List<Comment>> GetCommentsAsync(string reviewId);

        Task RateAsync(bool isPositive, string userId, string commentId);

        Task<bool?> GetUserRate(string userId, string commentId);
    }
}
