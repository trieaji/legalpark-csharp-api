using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Report
{
    public class AdminParkingSpotOccupancyReportResponse
    {
        [JsonPropertyName("spotId")]
        public string SpotId { get; set; }

        [JsonPropertyName("spotNumber")]
        public string SpotNumber { get; set; }

        [JsonPropertyName("spotType")]
        public string SpotType { get; set; }

        [JsonPropertyName("currentStatus")]
        public string CurrentStatus { get; set; }

        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; set; }

        [JsonPropertyName("merchantName")]
        public string MerchantName { get; set; }

        [JsonPropertyName("currentVehicleLicensePlate")]
        public string CurrentVehicleLicensePlate { get; set; }

        [JsonPropertyName("currentVehicleType")]
        public string CurrentVehicleType { get; set; }

        [JsonPropertyName("currentOccupantUserName")]
        public string CurrentOccupantUserName { get; set; }
    }
}
