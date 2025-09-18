using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.User
{
    public class UserBasicResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
