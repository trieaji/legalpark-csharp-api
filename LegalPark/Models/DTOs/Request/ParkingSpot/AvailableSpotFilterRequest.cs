using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingSpot
{
    public class AvailableSpotFilterRequest
    {
        
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Merchant Code must be between 1 and 50 characters")]
        public string MerchantCode { get; set; }

        
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Spot type must be between 3 and 20 characters (e.g., CAR, MOTORCYCLE)")]
        public string SpotType { get; set; }

        
        [Range(0, int.MaxValue, ErrorMessage = "Floor number cannot be negative")]
        public int? Floor { get; set; }
    }
}
