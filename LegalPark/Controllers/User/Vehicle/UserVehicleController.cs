using LegalPark.Models.DTOs.Request.Vehicle;
using LegalPark.Services.Vehicle.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.Vehicle
{
    /// <summary>
    /// Controller untuk mengelola endpoint Vehicle API.
    /// </summary>
    [ApiController]
    [Route("api/v1/user")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("Vehicle Controller", Description = "Vehicle Service")]
    public class UserVehicleController : ControllerBase
    {
        private readonly IUserVehicleService _userVehicleService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public UserVehicleController(IUserVehicleService userVehicleService)
        {
            _userVehicleService = userVehicleService;
        }

        /// <summary>
        /// Endpoint untuk pengguna mendaftarkan kendaraan baru.
        /// </summary>
        /// <param name="request">Request body berisi data kendaraan.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("vehicle/register")]
        public async Task<IActionResult> UserRegisterVehicle([FromBody] VehicleRequest request)
        {
            return await _userVehicleService.UserRegisterVehicle(request);
        }

        /// <summary>
        /// Endpoint untuk pengguna mengambil semua kendaraan mereka.
        /// </summary>
        /// <returns>Respons HTTP yang berisi daftar kendaraan.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("vehicles")]
        public async Task<IActionResult> UserGetAllVehicle()
        {
            return await _userVehicleService.UserGetAllVehicle();
        }

        /// <summary>
        /// Endpoint untuk pengguna mengambil detail kendaraan berdasarkan ID.
        /// </summary>
        /// <param name="id">ID kendaraan dari URL path.</param>
        /// <returns>Respons HTTP yang berisi detail kendaraan.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> UserGetVehicleById([FromRoute] string id)
        {
            return await _userVehicleService.UserGetVehicleById(id);
        }

        /// <summary>
        /// Endpoint untuk pengguna memperbarui data kendaraan.
        /// </summary>
        /// <param name="id">ID kendaraan dari URL path.</param>
        /// <param name="request">Request body berisi data kendaraan yang akan diperbarui.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpPatch("vehicle/{id}")]
        public async Task<IActionResult> UserUpdateVehicle([FromRoute] string id, [FromBody] VehicleUpdateRequest request)
        {
            return await _userVehicleService.UserUpdateVehicle(id, request);
        }
    }

}
