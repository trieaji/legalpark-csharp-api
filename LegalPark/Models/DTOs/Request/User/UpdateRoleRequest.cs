using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.User
{
    public class UpdateRoleRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
