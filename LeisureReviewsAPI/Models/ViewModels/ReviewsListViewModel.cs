using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.ViewModels
{
    public class ReviewsListViewModel : PagesViewModel
    {
        public List<Review> Reviews { get; set; }

        public ReviewSortModel ReviewSortModel { get; set; } = new();
    }
}
