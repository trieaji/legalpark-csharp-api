using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.VerificationCode
{
    public class VerifyPaymentCodeRequest
    {
        
        [Required(ErrorMessage = "User ID cannot be blank")]
        public string UserId { get; set; }

        
        [Required(ErrorMessage = "Verification code cannot be blank")]
        public string Code { get; set; }

        
        [Required(ErrorMessage = "Parking transaction ID cannot be empty")]
        public string ParkingTransactionId { get; set; }
    }
}
