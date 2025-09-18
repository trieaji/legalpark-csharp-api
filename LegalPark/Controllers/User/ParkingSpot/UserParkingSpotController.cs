using LegalPark.Models.DTOs.Request.ParkingSpot;
using LegalPark.Services.ParkingSpot.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.ParkingSpot
{
    /// <summary>
    /// Controller untuk mengelola endpoint User Parking Spot API.
    /// </summary>
    [ApiController]
    [Route("api/v1/user")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("User Parking Spot API", Description = "Endpoint untuk pengguna (User) melihat informasi slot parkir")]
    public class UserParkingSpotController : ControllerBase
    {
        private readonly IUserParkingSpotService _userParkingSpotService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public UserParkingSpotController(IUserParkingSpotService userParkingSpotService)
        {
            _userParkingSpotService = userParkingSpotService;
        }

        /// <summary>
        /// Mengambil daftar slot parkir yang tersedia berdasarkan filter.
        /// </summary>
        /// <param name="filter">Objek filter yang di-bind dari query parameters.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("parking-spots/available")]
        public async Task<IActionResult> GetAvailableParkingSpots([FromQuery] AvailableSpotFilterRequest filter)
        {
            // [FromQuery] digunakan untuk binding query parameters, setara dengan @ModelAttribute di Spring.
            // Validasi akan otomatis dipicu jika model memiliki anotasi validasi.
            return await _userParkingSpotService.UserGetAvailableParkingSpotsAsync(filter);
        }

        /// <summary>
        /// Mengambil semua slot parkir di suatu merchant berdasarkan kode merchant.
        /// </summary>
        /// <param name="merchantCode">Kode merchant dari URL path.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("parking-spots/by-merchant/{merchantCode}")]
        public async Task<IActionResult> GetParkingSpotsByMerchant([FromRoute] string merchantCode)
        {
            // [FromRoute] digunakan untuk binding path variable, setara dengan @PathVariable di Spring.
            return await _userParkingSpotService.UserGetParkingSpotsByMerchantAsync(merchantCode);
        }
    }

}
