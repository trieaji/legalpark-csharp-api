using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Report
{
    public class UserSummaryReportResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("totalParkingSessions")]
        public long TotalParkingSessions { get; set; }

        [JsonPropertyName("totalCostSpent")]
        public decimal TotalCostSpent { get; set; }

        [JsonPropertyName("currentBalance")]
        public decimal CurrentBalance { get; set; }
    }
}
