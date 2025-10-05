using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.User
{
    public class RegisterRequest
    {
        
        [Required(ErrorMessage = "Account name is required.")]
        [MaxLength(50)]
        public string? AccountName { get; set; }

        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string? Password { get; set; }
    }
}
