using System.ComponentModel.DataAnnotations;
namespace Scanner.Models
{
    public class PasswordReq
    {
        //[Required(ErrorMessage = "* Email is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Email Input (Max 50 letters)")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Msg { get; set; }
    }
}