using LeisureReviewsAPI.Data;

namespace LeisureReviewsAPI.Models
{
    public class ReviewSortModel
    {
        public ReviewSortTarget Target { get; set; } = ReviewSortTarget.Date;

        public SortType Type { get; set; } = SortType.Descending;
    }
}
