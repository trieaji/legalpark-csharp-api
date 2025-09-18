using LegalPark.Models.DTOs.Request.Merchant;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Merchant
{
    public interface IMerchantService
    {
        Task<IActionResult> CreateNewMerchant(MerchantRequest request);
        Task<IActionResult> GetAllMerchants();
        Task<IActionResult> DeleteMerchant(string id);
        Task<IActionResult> UpdateExistingMerchant(string id, MerchantRequest request);
        Task<IActionResult> GetMerchantByCode(MerchantRequest request);
    }
}
