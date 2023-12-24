using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Repositories.Interfaces
{
    public interface ITagsRepository
    {
        Task<List<Tag>> GetAsync();

        Task<ICollection<Tag>> GetAsync(IEnumerable<string> tagsNames);

        Task<List<TagWeightModel>> GetWeightsAsync();

        Task AddNewAsync(IEnumerable<string> tagsNames);
    }
}
