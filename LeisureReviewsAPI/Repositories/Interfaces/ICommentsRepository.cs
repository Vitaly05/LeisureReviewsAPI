using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface ICommentsRepository
    {
        Task SaveAsync(Comment comment);
    }
}
