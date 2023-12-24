using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models.Database;
using System.Linq.Expressions;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetAllAsync(string authorId);

        Task<Review> GetAsync(string id);

        Task<List<Review>> GetRelatedAsync(string reviewId, int count);

        Task<List<Review>> GetLatestAsync(Expression<Func<Review, bool>> predicate, SortType sortType, int page, int pageSize);

        Task<List<Review>> GetTopRatedAsync(Expression<Func<Review, bool>> predicate, SortType sortType, int page, int pageSize);

        Task<List<Review>> GetTopLikedAsync(Expression<Func<Review, bool>> predicate, SortType sortType, int page, int pageSize);

        Task<int> GetPagesCountAsync(int pageSize, Expression<Func<Review, bool>> predicate);

        Task SaveAsync(Review review);

        Task DeleteAsync(string id);
    }
}
