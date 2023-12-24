using LeisureReviewsAPI.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.ViewModels
{
    public class ReviewEditorViewModel : BaseViewModel
    {
        public string AuthorName { get; set; }

        public Review Review { get; set; }

        public List<Tag> Tags { get; set; }

        [Required(ErrorMessage = "The Leisure field is required")]
        public string LeisureName { get; set; }
    }
}
