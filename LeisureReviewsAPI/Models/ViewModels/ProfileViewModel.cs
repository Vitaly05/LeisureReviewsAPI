using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.ViewModels
{
    public class ProfileViewModel : ReviewsListViewModel
    {
        public User User { get; set; }

        public int LikesCount { get; set; }

        public bool CanEdit
        {
            get
            {
                if (CurrentUser is null) return false;
                return User.UserName == CurrentUser.UserName || CurrentUser.Roles.Contains("Admin");
            }
        }
    }
}
