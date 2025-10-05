using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Request.Vehicle
{
    public class VehicleRequest
    {
        
        [Required]
        public string LicensePlate { get; set; }

        
        [Required]
        public string Type { get; set; }

        
        [JsonIgnore]
        public string? OwnerId { get; set; }
    }
}
