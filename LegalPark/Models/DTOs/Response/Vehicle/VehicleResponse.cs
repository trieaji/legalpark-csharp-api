using LegalPark.Models.DTOs.Response.User;
using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.Vehicle
{
    public class VehicleResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("licensePlate")]
        public string LicensePlate { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("owner")]
        public UserBasicResponse Owner { get; set; }
    }
}
