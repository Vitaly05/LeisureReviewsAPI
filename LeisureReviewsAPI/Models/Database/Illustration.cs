using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.Database
{
    public class Illustration
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ReviewId { get; set; }

        public Review Review { get; set; }
    }
}
