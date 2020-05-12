using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class TwoFactorModel
    {
        [Required]
        public string Token { get; set; }
    }
}