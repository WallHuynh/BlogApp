using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Comment
    {
        [Key]
        public string CommentID { get; set; }
        public string PostID { get; set; }
        public Post Post { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? email { get; set; }

        [StringLength(int.MaxValue)]
        public string? Content { get; set; }

        [Display(Name = "Created date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }

        public Comment()
        {
            CommentID = Guid.NewGuid().ToString();
        }
    }
}
