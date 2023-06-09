using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Models
{
    public class PostViewModel
    {
        public List<Post>? PostData { get; set; }
        public SelectList? Categories { get; set; }
        public string? SelectedCategoryID { get; set; }
        public string? SearchString { get; set; }
    }
}
