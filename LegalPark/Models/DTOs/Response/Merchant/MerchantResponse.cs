using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Merchant
{
    public class MerchantResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; set; }

        [JsonPropertyName("merchantName")]
        public string MerchantName { get; set; }

        [JsonPropertyName("merchantAddress")]
        public string MerchantAddress { get; set; }

        [JsonPropertyName("contactPerson")]
        public string ContactPerson { get; set; }

        [JsonPropertyName("contactPhone")]
        public string ContactPhone { get; set; }
    }

}
