using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.ViewModels
{
    public class BaseViewModel
    {
        public bool IsAuthorized { get; set; } = false;

        public User CurrentUser { get; set; }

        public string AdditionalUrl { get; set; } = "";
    }
}
