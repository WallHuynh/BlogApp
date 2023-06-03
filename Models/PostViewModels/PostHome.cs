namespace BlogApp.Models
{
    public class PostHome
    {
        public ICollection<Post> FeaturedPosts { get; set; }
        public Post LastestPost { get; set; }
    }
}
