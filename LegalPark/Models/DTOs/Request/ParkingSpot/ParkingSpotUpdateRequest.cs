using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingSpot
{
    public class ParkingSpotUpdateRequest
    {
        /// <summary>
        /// Nomor spot parkir yang diperbarui.
        /// </summary>
        [StringLength(20, ErrorMessage = "Spot number cannot exceed 20 characters")]
        public string SpotNumber { get; set; }

        /// <summary>
        /// Nomor lantai yang diperbarui.
        /// </summary>
        public int? Floor { get; set; }

        /// <summary>
        /// Tipe spot yang diperbarui.
        /// </summary>
        public string SpotType { get; set; }

        /// <summary>
        /// Status spot yang diperbarui.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Kode merchant yang diperbarui.
        /// </summary>
        public string MerchantCode { get; set; }
    }
}
