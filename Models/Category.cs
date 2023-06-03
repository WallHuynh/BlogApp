using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Category
    {
        [Key]
        public string CategoryID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }
        public ICollection<Post> Posts { get; set; }
        public Category()
        {
            CategoryID = Guid.NewGuid().ToString();
        }
    }
}
