using LegalPark.Models.Entities;
using LegalPark.Services.Report.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegalPark.Controllers.Admin.Report
{
    
    [ApiController]
    [Route("api/v1/admin")]
    public class AdminReportController : ControllerBase
    {
        private readonly IAdminReportService _adminReportService;
        private readonly ILogger<AdminReportController> _logger;

        
        public AdminReportController(IAdminReportService adminReportService, ILogger<AdminReportController> logger)
        {
            _adminReportService = adminReportService;
            _logger = logger;
        }

        
        [Authorize(Roles = "Admin")] 
        [HttpGet("report/revenue/daily")]
        public async Task<IActionResult> GetDailyRevenueReport(
            [FromQuery] DateTime date, 
            [FromQuery(Name = "merchantCode")] string? merchantCode = null) 
        {
            _logger.LogInformation("Request for daily revenue report on {Date} for merchant {MerchantCode}", date.ToShortDateString(), merchantCode);
            return await _adminReportService.GetDailyRevenueReport(date, merchantCode);
        }

        
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
