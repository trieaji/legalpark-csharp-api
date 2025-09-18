using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Request.Vehicle
{
    public class VehicleRequest
    {
        /// <summary>
        /// Plat nomor kendaraan.
        /// </summary>
        [Required]
        public string LicensePlate { get; set; }

        /// <summary>
        /// Tipe kendaraan (misalnya, mobil, motor).
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// ID pemilik kendaraan.
        /// </summary>
        [JsonIgnore]
        public string? OwnerId { get; set; }
    }
}
