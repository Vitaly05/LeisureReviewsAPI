using LeisureReviewsAPI.Models.Database;
using Newtonsoft.Json;

namespace LeisureReviewsAPI.Models.Search
{
    public class ReviewSearchModel
    {
        [JsonProperty("objectID")]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Leisure { get; set; }

        public string Content { get; set; }

        public ICollection<CommentSearchModel> Comments { get; set; }

        public ICollection<TagSearchModel> Tags { get; set; }
    }
}
