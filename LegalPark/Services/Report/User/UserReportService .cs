using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Response.Report;
using LegalPark.Models.Entities;
using LegalPark.Repositories.ParkingTransaction;
using LegalPark.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Report.User
{
    public class UserReportService : IUserReportService
    {
        private readonly IUserRepository _userRepository;
        private readonly IParkingTransactionRepository _parkingTransactionRepository;
        private readonly ReportResponseMapper _reportResponseMapper;

        public UserReportService(
            IUserRepository userRepository,
            IParkingTransactionRepository parkingTransactionRepository,
            ReportResponseMapper reportResponseMapper)
        {
            _userRepository = userRepository;
            _parkingTransactionRepository = parkingTransactionRepository;
            _reportResponseMapper = reportResponseMapper;
        }

        public async Task<IActionResult> GetUserParkingHistory(Guid userId, DateTime startDate, DateTime endDate)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"User not found with ID: {userId}");
            }

            // Convert DateOnly ke DateTime (awal dan akhir hari)
            var startDateTime = startDate.Date; // jam 00:00
            var endDateTime = endDate.Date.AddDays(1).AddTicks(-1); // jam 23:59:59.9999999

            var transactions = await _parkingTransactionRepository
                .findByVehicleOwnerIdAndEntryTimeBetween(userId, startDateTime, endDateTime);

            if (transactions == null || !transactions.Any())
            {
                return ResponseHandler.GenerateResponseSuccess(new List<UserParkingHistoryReportResponse>());
            }

            var responses = transactions
                .Select(t => _reportResponseMapper.MapToUserParkingHistoryReportResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> GetUserSummaryReport(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"User not found with ID: {userId}");
            }

            var summary = new UserSummaryReportResponse
            {
                UserId = user.Id.ToString(),
                UserName = user.AccountName,
                CurrentBalance = user.Balance
            };

            var userTransactions = await _parkingTransactionRepository.findByVehicleOwnerId(userId);

            long totalSessions = userTransactions.Count;
            var totalCost = userTransactions
                .Where(t => t.PaymentStatus == PaymentStatus.PAID && t.TotalCost != null)
                .Select(t => t.TotalCost ?? 0)
                .Aggregate(0m, (acc, val) => acc + val);

            summary.TotalParkingSessions = totalSessions;
            summary.TotalCostSpent = totalCost;

            return ResponseHandler.GenerateResponseSuccess(summary);
        }
    }
}
