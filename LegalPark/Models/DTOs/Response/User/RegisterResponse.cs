using System.Text.Json.Serialization;

namespace LegalPark.Models.DTOs.Response.User
{
    public class RegisterResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
