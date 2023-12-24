using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface ILeisuresRepository
    {
        Task<Leisure> GetAsync(string id);

        Task<Leisure> GetFromReviewAsync(string reviewId);

        Task<Leisure> AddAsync(string name);
    }
}
