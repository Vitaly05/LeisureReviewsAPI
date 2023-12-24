using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.ViewModels
{
    public class AdminPanelViewModel : PagesViewModel
    {
        public List<User> Users { get; set; }
    }
}
