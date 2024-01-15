using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.Dto
{
    public class CommentDto
    {
        public string Id { get; set; }

        public string AuthorName { get; set; }

        public string Text { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public static class CommentExtension
    {
        public static CommentDto ConvertToDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                AuthorName = comment.Author.UserName,
                Text = comment.Text,
                CreateTime = comment.CreateTime,
            };
        }
    }
}
