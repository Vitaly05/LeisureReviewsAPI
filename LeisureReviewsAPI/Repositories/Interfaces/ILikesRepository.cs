using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface ILikesRepository
    {
        Task LikeAsync(Review review, User likedUser);

        Task<int> GetCountAsync(User user);
    }
}
