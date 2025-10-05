using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingSpot
{
    public class ParkingSpotUpdateRequest
    {
        
        [StringLength(20, ErrorMessage = "Spot number cannot exceed 20 characters")]
        public string SpotNumber { get; set; }

        
        public int? Floor { get; set; }

        
        public string SpotType { get; set; }

        
        public string Status { get; set; }

        
        public string MerchantCode { get; set; }
    }
}
