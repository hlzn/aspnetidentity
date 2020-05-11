using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}