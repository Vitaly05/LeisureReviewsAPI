using LeisureReviewsAPI.Data;

namespace LeisureReviewsAPI.Models
{
    public class ReviewSortModel
    {
        public ReviewSortTarget Target { get; set; } = ReviewSortTarget.Date;

        public LeisureGroup? LeisureGroup { get; set; } = null;

        public SortType Type { get; set; } = SortType.Descending;
    }
}
