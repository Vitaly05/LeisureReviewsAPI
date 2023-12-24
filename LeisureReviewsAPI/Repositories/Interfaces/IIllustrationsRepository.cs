using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface IIllustrationsRepository
    {
        Task AddAsync(string reviewId, IFormFile file);

        Task DeleteAllAsync(string reviewId);
    }
}
