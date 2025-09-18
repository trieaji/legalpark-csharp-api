using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Merchant
{
    public class MerchantRequest
    {
        [Required]
        public string MerchantName { get; set; }

        [Required]
        public string MerchantAddress { get; set; }

        [Required]
        public string ContactPerson { get; set; }

        [Required]
        public string ContactPhone { get; set; }

        public string? MerchantCode { get; set; }
    }
}
