using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Report
{
    public class AdminDailyRevenueReportResponse
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; set; }

        [JsonPropertyName("merchantName")]
        public string MerchantName { get; set; }

        [JsonPropertyName("totalRevenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("totalPaidTransactions")]
        public long TotalPaidTransactions { get; set; }

        [JsonPropertyName("totalFailedTransactions")]
        public long TotalFailedTransactions { get; set; }
    }
}
