using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class Tag
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = "";

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
