using Newtonsoft.Json;

namespace LeisureReviewsAPI.Models.Search
{
    public class LeisureSearchModel
    {
        [JsonProperty("objectID")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }
    }
}
