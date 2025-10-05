using LegalPark.Models.DTOs.Request.Merchant;
using LegalPark.Services.Merchant;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Merchant
{
    
    [ApiController]
    [Route("api/v1")]
    
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;

        
        public MerchantController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }

        
        [HttpPost("merchants")]
        public async Task<IActionResult> CreateMerchant([FromBody] MerchantRequest request)
        {
            return await _merchantService.CreateNewMerchant(request);
        }

        
        [HttpGet("merchants/find")]
        public async Task<IActionResult> GetAllMerchants()
        {
            return await _merchantService.GetAllMerchants();
        }

        
        [HttpPatch("merchants/update/{id}")]
        public async Task<IActionResult> UpdateMerchant([FromRoute] string id, [FromBody] MerchantRequest request)
        {
            return await _merchantService.UpdateExistingMerchant(id, request);
        }

        
        [HttpDelete("merchants/delete/{id}")]
        public async Task<IActionResult> DeleteMerchant([FromRoute] string id)
        {
            return await _merchantService.DeleteMerchant(id);
        }

        
        [HttpGet("merchants/get-by-code")]
        public async Task<IActionResult> GetMerchantByCode([FromBody] MerchantRequest request)
        {
            return await _merchantService.GetMerchantByCode(request);
        }
    }

}
