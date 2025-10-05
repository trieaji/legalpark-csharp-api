using LegalPark.Services.Report.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.Report
{
    
    [ApiController]
    [Route("api/v1/user")]
    
    public class UserReportController : ControllerBase
    {
        private readonly IUserReportService _userReportService;

        
        public UserReportController(IUserReportService userReportService)
        {
            _userReportService = userReportService;
        }

        
        [Authorize(Roles = "User, Admin")] 
        [HttpGet("report/{userId}/history")]
        public async Task<IActionResult> GetUserParkingHistory(
            [FromRoute] Guid userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            
            return await _userReportService.GetUserParkingHistory(userId, startDate, endDate);
        }

        
        [Authorize(Roles = "User, Admin")] 
        [HttpGet("report/{userId}/summary")]
        public async Task<IActionResult> GetUserSummaryReport([FromRoute] Guid userId)
        {
            
            return await _userReportService.GetUserSummaryReport(userId);
        }
    }

}
