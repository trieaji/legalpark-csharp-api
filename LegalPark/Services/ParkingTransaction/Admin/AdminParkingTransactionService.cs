using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Response.ParkingTransaction;
using LegalPark.Models.Entities;
using LegalPark.Repositories.Generic;
using LegalPark.Repositories.Merchant;
using LegalPark.Repositories.ParkingSpot;
using LegalPark.Repositories.ParkingTransaction;
using LegalPark.Repositories.Vehicle;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.ParkingTransaction.Admin
{
    public class AdminParkingTransactionService : IAdminParkingTransactionService
    {
        private readonly IParkingTransactionRepository _parkingTransactionRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IParkingSpotRepository _parkingSpotRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly ParkingTransactionResponseMapper _parkingTransactionResponseMapper;

        public AdminParkingTransactionService(
            IParkingTransactionRepository parkingTransactionRepository,
            IVehicleRepository vehicleRepository,
            IParkingSpotRepository parkingSpotRepository,
            IMerchantRepository merchantRepository,
            ParkingTransactionResponseMapper parkingTransactionResponseMapper)
        {
            _parkingTransactionRepository = parkingTransactionRepository;
            _vehicleRepository = vehicleRepository;
            _parkingSpotRepository = parkingSpotRepository;
            _merchantRepository = merchantRepository;
            _parkingTransactionResponseMapper = parkingTransactionResponseMapper;
        }

        public async Task<IActionResult> AdminGetAllParkingTransactions()
        {
            var transactions = await _parkingTransactionRepository.GetAllWithDetailsAsync();
            var responses = transactions
                .Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminGetParkingTransactionById(Guid transactionId)
        {
            var transaction = await _parkingTransactionRepository.GetByIdWithDetailsAsync(transactionId);
            if (transaction == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED",
                    $"Parking transaction not found with ID: {transactionId}");
            }

            return ResponseHandler.GenerateResponseSuccess(
                _parkingTransactionResponseMapper.MapToParkingTransactionResponse(transaction));
        }

        public async Task<IActionResult> AdminGetParkingTransactionsByVehicleId(Guid vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED",
                    $"Vehicle not found with ID: {vehicleId}");
            }

            var transactions = await _parkingTransactionRepository.findByVehicle(vehicle);
            var responses = transactions
                .Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminGetParkingTransactionsByParkingSpotId(Guid parkingSpotId)
        {
            var parkingSpot = await _parkingSpotRepository.GetByIdAsync(parkingSpotId);
            if (parkingSpot == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED",
                    $"Parking spot not found with ID: {parkingSpotId}");
            }

            var transactions = await _parkingTransactionRepository.findByParkingSpot(parkingSpot);
            var responses = transactions
                .Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminGetParkingTransactionsByMerchantId(Guid merchantId)
        {
            var merchant = await _merchantRepository.GetByIdAsync(merchantId);
            if (merchant == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED",
                    $"Merchant not found with ID: {merchantId}");
            }

            var transactions = await _parkingTransactionRepository.findByParkingSpot_Merchant(merchant);
            var responses = transactions
                .Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminGetParkingTransactionsByParkingStatus(ParkingStatus status)
        {
            var transactions = await _parkingTransactionRepository.findByStatus(status);
            var responses = transactions
                .Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminGetParkingTransactionsByPaymentStatus(PaymentStatus paymentStatus)
        {
            var transactions = await _parkingTransactionRepository.findByPaymentStatus(paymentStatus);
            var responses = transactions
                .Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t))
                .ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminUpdateParkingTransactionPaymentStatus(Guid transactionId, PaymentStatus newPaymentStatus)
        {
            var transaction = await _parkingTransactionRepository.UpdatePaymentStatusAsync(transactionId, newPaymentStatus);

            if (transaction == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED",
                    $"Parking transaction not found with ID: {transactionId}");
            }

            return ResponseHandler.GenerateResponseSuccess(
                _parkingTransactionResponseMapper.MapToParkingTransactionResponse(transaction));
        }

        public async Task<IActionResult> AdminCancelParkingTransaction(Guid transactionId)
        {
            var transaction = await _parkingTransactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED",
                    $"Parking transaction not found with ID: {transactionId}");
            }

            if (transaction.Status == ParkingStatus.COMPLETED)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                    "Completed transactions cannot be cancelled.");
            }

            if (transaction.Status == ParkingStatus.CANCELLED)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED",
                    "Transaction is already cancelled.");
            }

            if (transaction.Status == ParkingStatus.ACTIVE && transaction.ParkingSpot != null)
            {
                transaction.ParkingSpot.Status = ParkingSpotStatus.AVAILABLE;
                _parkingSpotRepository.Update(transaction.ParkingSpot);
                await _parkingSpotRepository.SaveChangesAsync();
            }

            transaction.Status = ParkingStatus.CANCELLED;
            _parkingTransactionRepository.Update(transaction);
            await _parkingTransactionRepository.SaveChangesAsync();

            return ResponseHandler.GenerateResponseSuccess(null);
        }
    }
}
