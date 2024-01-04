using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models
{
    public class AccountInfo
    {
        public bool IsAuthorized { get; set; } = false;

        public User CurrentUser { get; set; }
    }
}
