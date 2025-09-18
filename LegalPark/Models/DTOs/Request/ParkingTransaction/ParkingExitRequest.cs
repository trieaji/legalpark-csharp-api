using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingTransaction
{
    public class ParkingExitRequest
    {
        /// <summary>
        /// Plat nomor kendaraan.
        /// Properti ini wajib diisi dan memiliki batasan panjang.
        /// </summary>
        [Required(ErrorMessage = "License plate cannot be empty")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "License plate must be between 1 and 20 characters")]
        public string LicensePlate { get; set; }

        /// <summary>
        /// Kode merchant untuk parkir.
        /// Properti ini wajib diisi dan memiliki batasan panjang.
        /// </summary>
        [Required(ErrorMessage = "Merchant code cannot be empty")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Merchant code must be between 1 and 50 characters")]
        public string MerchantCode { get; set; }

        /// <summary>
        /// Kode verifikasi untuk validasi keluar.
        /// Properti ini wajib diisi dan memiliki batasan panjang.
        /// </summary>
        [Required(ErrorMessage = "Verification code cannot be empty")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "Verification code must be minimum 4 characters")]
        public string VerificationCode { get; set; }
    }
}
