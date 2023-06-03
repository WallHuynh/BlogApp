using BlogApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Post
    {
        [Key]
        public string PostID { get; set; }
        public string BlogUserID { get; set; }
        public BlogAppUser BlogAppUser { get; set; }

        public string Title { get; set; }
        public string Introduction { get; set; }

        [StringLength(int.MaxValue)]
        public string Content { get; set; }

        [Display(Name = "Published date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        [Compare(nameof(PublishedDate)), Display(Name = "Last update")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }
        public bool IsPublished { get; set; }
        public string CategoryID { get; internal set; }
        public Category? Category { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public string ImgUrl { get; set; }

        public Post()
        {
            // Generate a new GUID for the PostID
            PostID = Guid.NewGuid().ToString();
        }
    }
}
