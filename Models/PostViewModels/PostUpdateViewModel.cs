using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class PostUpdateViewModel
    {
        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Introduction { get; set; }

        [StringLength(int.MaxValue)]
        public string Content { get; set; }
        public bool IsPublished { get; set; }
        public string CategoryID { get; set; }

        // public string ImgUrl { get; set; }

        [DisplayName("Upload Image")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }
    }
}
