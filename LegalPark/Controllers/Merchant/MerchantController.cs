using LegalPark.Models.DTOs.Request.Merchant;
using LegalPark.Services.Merchant;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Merchant
{
    /// <summary>
    /// Controller untuk mengelola endpoint Merchant Service.
    /// </summary>
    [ApiController]
    [Route("api/v1")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("Merchant Controller", Description = "Merchant Service")]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public MerchantController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }

        /// <summary>
        /// Membuat merchant baru.
        /// </summary>
        /// <param name="request">Request body berisi data merchant.</param>
        /// <returns>Respons HTTP.</returns>
        [HttpPost("merchants")]
        public async Task<IActionResult> CreateMerchant([FromBody] MerchantRequest request)
        {
            return await _merchantService.CreateNewMerchant(request);
        }

        /// <summary>
        /// Mengambil semua merchant.
        /// </summary>
        /// <returns>Respons HTTP.</returns>
        [HttpGet("merchants/find")]
        public async Task<IActionResult> GetAllMerchants()
        {
            return await _merchantService.GetAllMerchants();
        }

        /// <summary>
        /// Memperbarui merchant yang sudah ada.
        /// </summary>
        /// <param name="id">ID merchant.</param>
        /// <param name="request">Request body berisi data merchant yang diperbarui.</param>
        /// <returns>Respons HTTP.</returns>
        [HttpPatch("merchants/update/{id}")]
        public async Task<IActionResult> UpdateMerchant([FromRoute] string id, [FromBody] MerchantRequest request)
        {
            return await _merchantService.UpdateExistingMerchant(id, request);
        }

        /// <summary>
        /// Menghapus merchant berdasarkan ID.
        /// </summary>
        /// <param name="id">ID merchant.</param>
        /// <returns>Respons HTTP.</returns>
        [HttpDelete("merchants/delete/{id}")]
        public async Task<IActionResult> DeleteMerchant([FromRoute] string id)
        {
            return await _merchantService.DeleteMerchant(id);
        }

        /// <summary>
        /// Mengambil merchant berdasarkan kode.
        /// </summary>
        /// <param name="request">Request body berisi kode merchant.</param>
        /// <returns>Respons HTTP.</returns>
        [HttpGet("merchants/get-by-code")]
        public async Task<IActionResult> GetMerchantByCode([FromBody] MerchantRequest request)
        {
            return await _merchantService.GetMerchantByCode(request);
        }
    }

}
