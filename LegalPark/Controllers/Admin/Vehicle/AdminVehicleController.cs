using LegalPark.Models.DTOs.Request.Vehicle;
using LegalPark.Services.Vehicle.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Admin.Vehicle
{
    /// <summary>
    /// Controller untuk mengelola endpoint Vehicle Management oleh Admin.
    /// </summary>
    // [ApiController] - Menandakan bahwa ini adalah controller API
    // [Route("api/v1/admin")] - Menentukan base URL untuk controller ini
    // [Tag] - Menggunakan namespace Swashbuckle.AspNetCore.Annotations untuk tag Swagger
    [ApiController]
    [Route("api/v1/admin")]
    // [Tag(name = "Admin Vehicle API", description = "Endpoint for Vehicle Management by Admin")] // Swagger
    public class AdminVehicleController : ControllerBase
    {
        private readonly IAdminVehicleService _adminVehicleService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public AdminVehicleController(IAdminVehicleService adminVehicleService)
        {
            _adminVehicleService = adminVehicleService;
        }

        /// <summary>
        /// Mendaftarkan kendaraan baru.
        /// </summary>
        /// <param name="request">Request body berisi data kendaraan.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("vehicle/register")]
        public async Task<IActionResult> AdminRegisterVehicle([FromBody] VehicleRequest request) // [FromBody] untuk @RequestBody
        {
            return await _adminVehicleService.AdminRegisterVehicle(request);
        }

        /// <summary>
        /// Mengambil semua kendaraan.
        /// </summary>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("vehicles")]
        public async Task<IActionResult> AdminGetAllVehicles()
        {
            return await _adminVehicleService.AdminGetAllVehicles();
        }

        /// <summary>
        /// Mengambil kendaraan berdasarkan ID.
        /// </summary>
        /// <param name="id">ID kendaraan.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> AdminGetVehicleById([FromRoute] string id) // [FromRoute] untuk @PathVariable
        {
            return await _adminVehicleService.AdminGetVehicleById(id);
        }

        /// <summary>
        /// Mengambil kendaraan berdasarkan User ID.
        /// </summary>
        /// <param name="userId">ID pengguna.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("vehicle/by-user/{userId}")]
        public async Task<IActionResult> AdminGetVehiclesByUserId([FromRoute] string userId)
        {
            return await _adminVehicleService.AdminGetVehiclesByUserId(userId);
        }

        /// <summary>
        /// Menghapus kendaraan berdasarkan ID.
        /// </summary>
        /// <param name="id">ID kendaraan.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("vehicle/{id}")]
        public async Task<IActionResult> AdminDeleteVehicle([FromRoute] string id)
        {
            return await _adminVehicleService.AdminDeleteVehicle(id);
        }
    }
}
