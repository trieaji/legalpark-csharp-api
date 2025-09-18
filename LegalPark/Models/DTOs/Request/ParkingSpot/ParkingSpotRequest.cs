using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingSpot
{
    public class ParkingSpotRequest
    {
        /// <summary>
        /// Nomor spot parkir, misalnya "A01", "B-P05".
        /// </summary>
        [Required(ErrorMessage = "Spot number is required")]
        [StringLength(20, ErrorMessage = "Spot number cannot exceed 20 characters")]
        public string SpotNumber { get; set; }

        /// <summary>
        /// Nomor lantai spot parkir (opsional).
        /// Menggunakan 'int?' (nullable int) karena properti ini bisa bernilai null.
        /// </summary>
        public int? Floor { get; set; }

        /// <summary>
        /// Tipe spot parkir, misalnya "CAR", "MOTORCYCLE", "UNIVERSAL".
        /// </summary>
        [Required(ErrorMessage = "Spot type is required (e.g., CAR, MOTORCYCLE, UNIVERSAL)")]
        public string SpotType { get; set; }

        /// <summary>
        /// Kode merchant yang mengasosiasikan spot parkir.
        /// </summary>
        [Required(ErrorMessage = "Merchant code is required to associate the parking spot")]
        public string MerchantCode { get; set; }
    }
}
