using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Vehicle
{
    public class VehicleUpdateRequest
    {
        /// <summary>
        /// Plat nomor kendaraan.
        /// </summary>
        [StringLength(20, ErrorMessage = "License plate cannot exceed 20 characters")]
        public string? LicensePlate { get; set; }

        /// <summary>
        /// Tipe kendaraan (misalnya, mobil, motor).
        /// </summary>
        [StringLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters")]
        public string? Type { get; set; }

        /// <summary>
        /// Kode merchant.
        /// </summary>
        public string? MerchantCode { get; set; }

        /// <summary>
        /// ID pemilik kendaraan.
        /// </summary>
        public string? OwnerId { get; set; }
    }
}
