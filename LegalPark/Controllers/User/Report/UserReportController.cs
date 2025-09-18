using LegalPark.Services.Report.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.Report
{
    /// <summary>
    /// Controller untuk mengelola endpoint User Parking Report API.
    /// </summary>
    [ApiController]
    [Route("api/v1/user")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("User Parking Report API", Description = "Endpoint untuk Pengguna melihat report parkir mereka")]
    public class UserReportController : ControllerBase
    {
        private readonly IUserReportService _userReportService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public UserReportController(IUserReportService userReportService)
        {
            _userReportService = userReportService;
        }

        /// <summary>
        /// Mengambil riwayat parkir pengguna dalam rentang tanggal tertentu.
        /// </summary>
        /// <param name="userId">ID pengguna dari URL path.</param>
        /// <param name="startDate">Tanggal awal untuk filter dari query string.</param>
        /// <param name="endDate">Tanggal akhir untuk filter dari query string.</param>
        /// <returns>Respons HTTP yang berisi riwayat parkir.</returns>
        [Authorize(Roles = "User, Admin")] // Setara dengan @PreAuthorize("hasRole('USER') or hasRole('ADMIN')")
        [HttpGet("report/{userId}/history")]
        public async Task<IActionResult> GetUserParkingHistory(
            [FromRoute] Guid userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            // [FromRoute] untuk binding parameter dari URL path.
            // [FromQuery] untuk binding parameter dari query string.
            // Logika minimal, langsung panggil service
            return await _userReportService.GetUserParkingHistory(userId, startDate, endDate);
        }

        /// <summary>
        /// Mengambil laporan ringkasan parkir pengguna.
        /// </summary>
        /// <param name="userId">ID pengguna dari URL path.</param>
        /// <returns>Respons HTTP yang berisi laporan ringkasan.</returns>
        [Authorize(Roles = "User, Admin")] // Setara dengan @PreAuthorize("hasRole('USER') or hasRole('ADMIN')")
        [HttpGet("report/{userId}/summary")]
        public async Task<IActionResult> GetUserSummaryReport([FromRoute] Guid userId)
        {
            // [FromRoute] untuk binding parameter dari URL path.
            // Logika minimal, langsung panggil service
            return await _userReportService.GetUserSummaryReport(userId);
        }
    }

}
