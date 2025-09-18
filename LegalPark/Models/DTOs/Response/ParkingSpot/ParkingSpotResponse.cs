using LegalPark.Models.DTOs.Response.Merchant;
using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.ParkingSpot
{
    public class ParkingSpotResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("spotNumber")]
        public string SpotNumber { get; set; }

        [JsonPropertyName("spotType")]
        public string SpotType { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("floor")]
        public int Floor { get; set; }

        [JsonPropertyName("merchant")]
        public MerchantResponse Merchant { get; set; }
    }
}
