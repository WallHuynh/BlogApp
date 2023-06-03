using BlogApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BlogAppUser class
public class BlogAppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Post> Posts { get; set; }
}
