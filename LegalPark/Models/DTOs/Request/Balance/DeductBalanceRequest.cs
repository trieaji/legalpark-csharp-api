using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Balance
{
    public class DeductBalanceRequest
    {
        
        [Required(ErrorMessage = "User ID cannot be blank")]
        public string UserId { get; set; }

        
        [Required(ErrorMessage = "Amount cannot be null")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }
    }
}
