using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.VerificationCode
{
    public class PaymentVerificationCodeRequest
    {
        /// <summary>
        /// ID pengguna.
        /// </summary>
        [Required(ErrorMessage = "User ID cannot be blank")]
        public string UserId { get; set; }

        /// <summary>
        /// ID transaksi parkir.
        /// </summary>
        [Required(ErrorMessage = "Parking transaction ID cannot be empty")]
        public string ParkingTransactionId { get; set; }
    }
}
