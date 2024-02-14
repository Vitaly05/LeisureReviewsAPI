using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.Database
{
    public class CommentRate
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public bool IsPositive { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public string CommentId { get; set; }

        public Comment Comment { get; set; }
    }
}
