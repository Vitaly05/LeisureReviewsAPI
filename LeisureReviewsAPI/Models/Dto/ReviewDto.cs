using LeisureReviewsAPI.Data;
using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.Dto
{
    public class ReviewDto
    {
        public ReviewDto() { }
        public ReviewDto(Review review, bool fullInfo = false)
        {
            this.Id = review.Id;
            this.AuthorId = review.AuthorId;
            this.Title = review.Title;
            this.LeisureId = review.LeisureId;
            this.Group = review.Group;
            this.AuthorRate = review.AuthorRate;
            this.Tags = review.Tags.Select(t => t.Name).ToList();
            this.LikesCount = review.Likes.Count;
            this.CreateTime = review.CreateTime;
            if (fullInfo)
                this.Content = review.Content;
        }

        public string Id { get; set; }

        public string AuthorId { get; set; }

        public string Title { get; set; }

        public string LeisureId { get; set; }

        public LeisureGroup Group { get; set; }

        public byte AuthorRate { get; set; }

        public List<string> Tags { get; set; }

        public int LikesCount { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public static class ReviewExtension
    {
        public static Review ConvertToReview(this ReviewDto reviewDto, ICollection<Tag> tags, Leisure leisure)
        {
            return new Review
            {
                Id = reviewDto.Id,
                AuthorId = reviewDto.AuthorId,
                Title = reviewDto.Title,
                LeisureId = leisure.Id,
                Group = reviewDto.Group,
                AuthorRate = reviewDto.AuthorRate,
                Content = reviewDto.Content,
                Tags = tags,
                Leisure = leisure
            };
        }
    }
}
