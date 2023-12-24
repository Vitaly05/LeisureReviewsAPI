using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface IRatesRepository
    {
        Task<Rate> GetAsync(User user, Leisure leisure);

        Task<double> GetAverageRateAsync(Leisure leisure);

        Task SaveAsync(Rate rate);
    }
}
