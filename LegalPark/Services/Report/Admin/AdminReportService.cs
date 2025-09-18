using LegalPark.Exception;
using LegalPark.Models.DTOs.Response.Report;
using LegalPark.Models.Entities;
using LegalPark.Repositories.Merchant;
using LegalPark.Repositories.ParkingSpot;
using LegalPark.Repositories.ParkingTransaction;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Report.Admin
{
    public class AdminReportService : IAdminReportService
    {
        private readonly IParkingTransactionRepository _parkingTransactionRepository;
        private readonly IParkingSpotRepository _parkingSpotRepository;
        private readonly IMerchantRepository _merchantRepository;

        public AdminReportService(
            IParkingTransactionRepository parkingTransactionRepository,
            IParkingSpotRepository parkingSpotRepository,
            IMerchantRepository merchantRepository)
        {
            _parkingTransactionRepository = parkingTransactionRepository;
            _parkingSpotRepository = parkingSpotRepository;
            _merchantRepository = merchantRepository;
        }

        public async Task<IActionResult> GetDailyRevenueReport(DateTime date, string? merchantCode)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            List<LegalPark.Models.Entities.ParkingTransaction> paidTransactions = new();

            if (!string.IsNullOrEmpty(merchantCode))
            {
                var merchant = await _merchantRepository.FindByMerchantCodeAsync(merchantCode);
                if (merchant == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {merchantCode}");
                }

                paidTransactions = await _parkingTransactionRepository.findPaidTransactionsByExitTimeBetweenAndMerchantCode(startOfDay, endOfDay, merchantCode);
            }
            else
            {
                paidTransactions = await _parkingTransactionRepository.findPaidTransactionsByExitTimeBetween(startOfDay, endOfDay);
            }

            if (paidTransactions.Count == 0)
            {
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK,
                    $"No revenue recorded for {date:yyyy-MM-dd}" + (!string.IsNullOrEmpty(merchantCode) ? $" at merchant {merchantCode}" : "") + ".",
                    new List<object>());
            }

            if (string.IsNullOrEmpty(merchantCode))
            {
                // Grouping by merchant
                var transactionsByMerchant = paidTransactions
                    .Where(t => t.ParkingSpot != null && t.ParkingSpot.Merchant != null)
                    .GroupBy(t => t.ParkingSpot.Merchant.MerchantCode);

                var responses = new List<AdminDailyRevenueReportResponse>();

                foreach (var group in transactionsByMerchant)
                {
                    var merchant = group.First().ParkingSpot.Merchant;
                    var totalRevenue = group.Where(t => t.TotalCost != null)
                                            .Sum(t => t.TotalCost ?? 0);
                    responses.Add(CreateDailyRevenueResponse(date, merchant.MerchantCode, merchant.MerchantName, totalRevenue, group.Count()));
                }

                return ResponseHandler.GenerateResponseSuccess(responses);
            }
            else
            {
                var totalRevenue = paidTransactions.Where(t => t.TotalCost != null)
                                                   .Sum(t => t.TotalCost ?? 0);
                var merchantName = paidTransactions.First().ParkingSpot.Merchant.MerchantName;

                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK,
                    $"Daily revenue report retrieved successfully for merchant {merchantName}.",
                    new List<AdminDailyRevenueReportResponse>
                    {
                        CreateDailyRevenueResponse(date, merchantCode, merchantName, totalRevenue, paidTransactions.Count)
                    });
            }
        }

        private AdminDailyRevenueReportResponse CreateDailyRevenueResponse(
            DateTime date, string merchantCode, string merchantName, decimal totalRevenue, long totalPaidTransactions)
        {
            return new AdminDailyRevenueReportResponse
            {
                Date = date,
                MerchantCode = merchantCode,
                MerchantName = merchantName,
                TotalRevenue = totalRevenue,
                TotalPaidTransactions = totalPaidTransactions,
                TotalFailedTransactions = 0 // kalau mau dihitung bisa ditambahkan logika
            };
        }

        public async Task<IActionResult> GetParkingSpotOccupancyReport(string? merchantCode, string? status)
        {
            List<LegalPark.Models.Entities.ParkingSpot> parkingSpots;

            if (!string.IsNullOrEmpty(merchantCode))
            {
                var merchant = await _merchantRepository.FindByMerchantCodeAsync(merchantCode);
                if (merchant == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {merchantCode}");
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (!Enum.TryParse(status, true, out ParkingSpotStatus spotStatus))
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Invalid parking spot status: {status}");
                    }
                    parkingSpots = await _parkingSpotRepository.findByMerchantAndStatus(merchant, spotStatus);
                }
                else
                {
                    parkingSpots = await _parkingSpotRepository.findByMerchant(merchant);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(status))
                {
                    if (!Enum.TryParse(status, true, out ParkingSpotStatus spotStatus))
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Invalid parking spot status: {status}");
                    }
                    parkingSpots = await _parkingSpotRepository.findByStatus(spotStatus);
                }
                else
                {
                    parkingSpots = (await _parkingSpotRepository.GetAllAsync()).ToList();
                }
            }

            if (!parkingSpots.Any())
            {
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "No parking spots found matching criteria.", new List<object>());
            }

            var responses = new List<AdminParkingSpotOccupancyReportResponse>();

            foreach (var spot in parkingSpots)
            {
                var response = new AdminParkingSpotOccupancyReportResponse
                {
                    SpotId = spot.Id.ToString(),
                    SpotNumber = spot.SpotNumber,
                    SpotType = spot.SpotType.ToString(),
                    CurrentStatus = spot.Status.ToString(),
                    MerchantCode = spot.Merchant?.MerchantCode,
                    MerchantName = spot.Merchant?.MerchantName
                };

                if (spot.Status == ParkingSpotStatus.OCCUPIED)
                {
                    var activeTransaction = await _parkingTransactionRepository.findByParkingSpotAndStatus(spot, ParkingStatus.ACTIVE);
                    if (activeTransaction != null && activeTransaction.Vehicle != null)
                    {
                        response.CurrentVehicleLicensePlate = activeTransaction.Vehicle.LicensePlate;
                        response.CurrentVehicleType = activeTransaction.Vehicle.Type.ToString();
                        if (activeTransaction.Vehicle.Owner != null)
                        {
                            response.CurrentOccupantUserName = activeTransaction.Vehicle.Owner.AccountName;
                        }
                    }
                }

                responses.Add(response);
            }

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK,
                "Parking spot occupancy report retrieved successfully.", responses);
        }
    }
}
