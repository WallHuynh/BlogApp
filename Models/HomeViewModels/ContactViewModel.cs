using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class ContactViewModel
    {
        [Required]
        [Display(Name = "First name")]
        [MaxLength(50, ErrorMessage = "Your First name only can be below 50 letters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [MaxLength(50, ErrorMessage = "Your First name only can be below 50 letters")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [MaxLength(600, ErrorMessage = "Sorry, your message must under 600 letters")]
        public string Message { get; set; }
    }
}
