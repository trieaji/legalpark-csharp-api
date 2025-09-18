using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Report.Admin
{
    public interface IAdminReportService
    {
        Task<IActionResult> GetDailyRevenueReport(DateTime date, string? merchantCode);
        Task<IActionResult> GetParkingSpotOccupancyReport(string? merchantCode, string? status);
    }
}
