using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.Database
{
    public class Comment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public User Author { get; set; }

        [Required]
        public Review Review { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public ICollection<CommentRate> Rates { get; set; } = new List<CommentRate>();
    }
}
