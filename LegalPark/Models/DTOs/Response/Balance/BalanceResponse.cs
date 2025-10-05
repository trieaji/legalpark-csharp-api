using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Balance
{
    public class BalanceResponse
    {
        
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        
        [JsonPropertyName("currentBalance")]
        public decimal CurrentBalance { get; set; }

        
        [JsonPropertyName("status")]
        public string Status { get; set; }

        
        [JsonPropertyName("message")]
        public string Message { get; set; }

        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
