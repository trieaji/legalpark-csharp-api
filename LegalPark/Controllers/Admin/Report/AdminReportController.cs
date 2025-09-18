using LegalPark.Models.Entities;
using LegalPark.Services.Report.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegalPark.Controllers.Admin.Report
{
    // Annotations:
    // [ApiController] - Menandakan bahwa ini adalah controller API
    // [Route("api/v1/admin")] - Menentukan base URL untuk controller ini
    [ApiController]
    [Route("api/v1/admin")]
    public class AdminReportController : ControllerBase
    {
        private readonly IAdminReportService _adminReportService;
        private readonly ILogger<AdminReportController> _logger;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public AdminReportController(IAdminReportService adminReportService, ILogger<AdminReportController> logger)
        {
            _adminReportService = adminReportService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint untuk melihat laporan pendapatan harian.
        /// Contoh: GET /api/v1/admin/report/revenue/daily?date=2024-01-01&merchantCode=merchant-a
        /// </summary>
        /// <param name="date">Tanggal laporan pendapatan (wajib).</param>
        /// <param name="merchantCode">Kode merchant (opsional).</param>
        /// <returns>Respons HTTP yang berisi laporan.</returns>
        [Authorize(Roles = "Admin")] // Contoh otorisasi, mirip dengan @PreAuthorize di Java
        [HttpGet("report/revenue/daily")]
        public async Task<IActionResult> GetDailyRevenueReport(
            [FromQuery] DateTime date, // Menggunakan DateTime untuk LocalDate
            [FromQuery(Name = "merchantCode")] string? merchantCode = null) // [FromQuery] untuk @RequestParam
        {
            _logger.LogInformation("Request for daily revenue report on {Date} for merchant {MerchantCode}", date.ToShortDateString(), merchantCode);
            return await _adminReportService.GetDailyRevenueReport(date, merchantCode);
        }

        /// <summary>
        /// Endpoint untuk melihat laporan hunian tempat parkir.
        /// Contoh: GET /api/v1/admin/report/occupancy?merchantCode=merchant-a&status=Occupied
        /// </summary>
        /// <param name="merchantCode">Kode merchant (opsional).</param>
        /// <param name="status">Status hunian (opsional, dari enum ParkingStatus).</param>
        /// <returns>Respons HTTP yang berisi laporan.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("report/occupancy")]
        public async Task<IActionResult> GetParkingSpotOccupancyReport(
            [FromQuery(Name = "merchantCode")] string merchantCode = null,
            [FromQuery(Name = "status")] string? status = null)
        {
            _logger.LogInformation("Request for parking spot occupancy report for merchant {MerchantCode} with status {Status}", merchantCode, status);
            return await _adminReportService.GetParkingSpotOccupancyReport(merchantCode, status);
        }
    }

}
