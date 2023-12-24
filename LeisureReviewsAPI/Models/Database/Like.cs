using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.Database
{
    public class Like
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public User User { get; set; }

        [Required]
        public Review Review { get; set; }
    }
}
