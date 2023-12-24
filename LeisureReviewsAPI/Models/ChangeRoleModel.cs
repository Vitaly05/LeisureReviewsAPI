using LeisureReviewsAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models
{
    public class ChangeRoleModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public Roles Role { get; set; }
    }
}
