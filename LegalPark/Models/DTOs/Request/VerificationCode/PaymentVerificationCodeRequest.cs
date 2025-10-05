using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.VerificationCode
{
    public class PaymentVerificationCodeRequest
    {
        
        [Required(ErrorMessage = "User ID cannot be blank")]
        public string UserId { get; set; }

        
        [Required(ErrorMessage = "Parking transaction ID cannot be empty")]
        public string ParkingTransactionId { get; set; }
    }
}
