using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Balance
{
    public class BalanceResponse
    {
        /// <summary>
        /// ID pengguna.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Saldo pengguna saat ini.
        /// </summary>
        [JsonPropertyName("currentBalance")]
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Status respons (misalnya, "SUCCESS", "FAILED").
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Pesan respons (misalnya, "Balance retrieved successfully").
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Timestamp respons.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
