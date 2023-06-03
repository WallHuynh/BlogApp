using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Contact
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
