using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.Dto
{
    public class UserDto
    {
        public UserDto() { }
        public UserDto(User user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.LikesCount = user.LikesCount;
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public int LikesCount { get; set; }
    }
}
