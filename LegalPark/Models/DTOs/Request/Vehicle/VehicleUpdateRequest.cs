using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Vehicle
{
    public class VehicleUpdateRequest
    {
        
        [StringLength(20, ErrorMessage = "License plate cannot exceed 20 characters")]
        public string? LicensePlate { get; set; }

        
        [StringLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters")]
        public string? Type { get; set; }

        
        public string? MerchantCode { get; set; }

        
        public string? OwnerId { get; set; }
    }
}
