using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.ParkingSpot
{
    public class ParkingSpotRequest
    {
        
        [Required(ErrorMessage = "Spot number is required")]
        [StringLength(20, ErrorMessage = "Spot number cannot exceed 20 characters")]
        public string SpotNumber { get; set; }

        
        public int? Floor { get; set; }

        
        [Required(ErrorMessage = "Spot type is required (e.g., CAR, MOTORCYCLE, UNIVERSAL)")]
        public string SpotType { get; set; }

        
        [Required(ErrorMessage = "Merchant code is required to associate the parking spot")]
        public string MerchantCode { get; set; }
    }
}
