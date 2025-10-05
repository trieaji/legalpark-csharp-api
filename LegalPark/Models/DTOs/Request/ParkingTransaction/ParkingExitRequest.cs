using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingTransaction
{
    public class ParkingExitRequest
    {
        
        [Required(ErrorMessage = "License plate cannot be empty")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "License plate must be between 1 and 20 characters")]
        public string LicensePlate { get; set; }

        
        [Required(ErrorMessage = "Merchant code cannot be empty")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Merchant code must be between 1 and 50 characters")]
        public string MerchantCode { get; set; }

        
        [Required(ErrorMessage = "Verification code cannot be empty")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "Verification code must be minimum 4 characters")]
        public string VerificationCode { get; set; }
    }
}
