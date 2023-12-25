using LeisureReviewsAPI.Models.Dto;

namespace LeisureReviewsAPI.Models
{
    public class AccountInfo
    {
        public bool IsAuthorized { get; set; } = false;

        public UserDto CurrentUser { get; set; }
    }
}
