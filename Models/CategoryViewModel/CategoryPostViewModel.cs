namespace BlogApp.Models
{
    public class CategoryPostViewModel
    {
        public ICollection<Post>? PostData { get; set; }
        public Category SelectedCategory { get; set; }
    }
}
