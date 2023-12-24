using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.ViewModels
{
    public class ReviewCardViewModel
    {
        public bool CanEdit { get; set; }

        public Review Review { get; set; }
    }
}
