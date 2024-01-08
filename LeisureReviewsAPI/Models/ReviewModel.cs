using LeisureReviewsAPI.Models.Dto;

namespace LeisureReviewsAPI.Models
{
    public class ReviewModel : ReviewDto
    {
        public List<string> TagsNames { get; set; }

        public string LeisureName { get; set; }

        public List<IFormFile> IllustrationsFiles { get; set; } = new List<IFormFile>();

        public bool IllustrationChanged { get; set; } = false;
    }
}
