using System.ComponentModel.DataAnnotations;
namespace Scanner.Models
{
    public class Contact
    {
        [Required(ErrorMessage = "* Contact Name is null")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "* Contact Name Input (Max 30 letters)")]
        [Display(Name = "Contact Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Mobile is null")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Mobile Input (Max 20 letters)")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "* Email is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Email Input (Max 50 letters)")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* Subject is null")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "* Subject (Max 200 letters)")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "* Email Body is null")]
        [Display(Name = "Email Body")]
        public string EmailBody { get; set; }

        [Display(Name = "Attachment")]
        public string Attachment { get; set; }
    }
}