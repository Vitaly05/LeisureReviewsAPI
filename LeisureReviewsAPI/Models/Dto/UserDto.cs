using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.Dto
{
    public class UserDto
    {
        public UserDto() { }
        public UserDto(User user)
        {
            this.UserName = user.UserName;
        }

        public string UserName { get; set; }
    }
}
