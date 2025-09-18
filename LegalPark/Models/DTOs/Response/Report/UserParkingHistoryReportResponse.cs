using LegalPark.Models.DTOs.Response.ParkingSpot;
using LegalPark.Models.DTOs.Response.Vehicle;
using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Report
{
    public class UserParkingHistoryReportResponse
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("entryTime")]
        public DateTime EntryTime { get; set; }

        [JsonPropertyName("exitTime")]
        public DateTime? ExitTime { get; set; } // Menggunakan tipe nullable karena mungkin belum keluar

        [JsonPropertyName("totalCost")]
        public decimal TotalCost { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonPropertyName("vehicle")]
        public VehicleResponse Vehicle { get; set; }

        [JsonPropertyName("parkingSpot")]
        public ParkingSpotResponse ParkingSpot { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
