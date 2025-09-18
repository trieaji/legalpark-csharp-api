using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Report.User
{
    public interface IUserReportService
    {
        Task<IActionResult> GetUserParkingHistory(Guid userId, DateTime startDate, DateTime endDate);

        Task<IActionResult> GetUserSummaryReport(Guid userId);
    }
}
