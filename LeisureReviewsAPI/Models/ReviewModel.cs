using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models
{
    public class ReviewModel : Review
    {
        public List<string> TagsNames { get; set; }

        public string LeisureName { get; set; }

        public List<IFormFile> IllustrationsFiles { get; set; }

        public bool IllustrationChanged { get; set; }
    }
}
