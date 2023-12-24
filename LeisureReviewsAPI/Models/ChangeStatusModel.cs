using LeisureReviewsAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models
{
    public class ChangeStatusModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public AccountStatus Status { get; set; }
    }
}
