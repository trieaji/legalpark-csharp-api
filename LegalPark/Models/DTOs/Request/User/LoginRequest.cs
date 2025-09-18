using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.User
{
    public class LoginRequest
    {
        // Properti Email untuk login.
        // Anotasi [Required] memastikan properti ini tidak boleh kosong saat validasi.
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        // Properti Password untuk login.
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
