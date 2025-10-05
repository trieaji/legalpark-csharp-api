using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.ParkingTransaction;
using LegalPark.Models.DTOs.Response.Merchant;
using LegalPark.Models.DTOs.Response.ParkingSpot;
using LegalPark.Models.DTOs.Response.ParkingTransaction;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.DTOs.Response.Vehicle;
using LegalPark.Models.Entities;
using LegalPark.Repositories.Merchant;
using LegalPark.Repositories.ParkingSpot;
using LegalPark.Repositories.ParkingTransaction;
using LegalPark.Repositories.User;
using LegalPark.Repositories.Vehicle;
using LegalPark.Services.Notification;
using LegalPark.Services.Payment;
using LegalPark.Services.Template;
using LegalPark.Util;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.ParkingTransaction.User
{
    public class UserParkingTransactionService : IUserParkingTransactionService
    {
        
        private readonly ILogger<UserParkingTransactionService> _logger;
        private readonly IParkingTransactionRepository _parkingTransactionRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IParkingSpotRepository _parkingSpotRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentService _paymentService;
        private readonly ParkingTransactionResponseMapper _parkingTransactionResponseMapper;
        private readonly INotificationService _notificationService;
        private readonly ITemplateService _templateService;

        
        public UserParkingTransactionService(
            ILogger<UserParkingTransactionService> logger,
            IParkingTransactionRepository parkingTransactionRepository,
            IVehicleRepository vehicleRepository,
            IParkingSpotRepository parkingSpotRepository,
            IMerchantRepository merchantRepository,
            IUserRepository userRepository,
            IPaymentService paymentService,
            ParkingTransactionResponseMapper parkingTransactionResponseMapper,
            INotificationService notificationService,
            ITemplateService templateService)
        {
            _logger = logger;
            _parkingTransactionRepository = parkingTransactionRepository;
            _vehicleRepository = vehicleRepository;
            _parkingSpotRepository = parkingSpotRepository;
            _merchantRepository = merchantRepository;
            _userRepository = userRepository;
            _paymentService = paymentService;
            _parkingTransactionResponseMapper = parkingTransactionResponseMapper;
            _notificationService = notificationService;
            _templateService = templateService;
        }

        
        
        public async Task<IActionResult> RecordParkingEntry(ParkingEntryRequest request)
        {
            _logger.LogInformation("Processing parking entry request for license plate: {LicensePlate}", request.LicensePlate);

            // 1. Search for vehicles by license plate number.
            var vehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not found.", request.LicensePlate);
                
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{request.LicensePlate}' not registered.");
            }

            // 2. Search for merchants by merchant code.
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);
            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found with code: {MerchantCode}", request.MerchantCode);
                
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {request.MerchantCode}.");
            }

            // 3. Check whether the vehicle already has an active transaction.
            var activeTransaction = await _parkingTransactionRepository.findByVehicleAndStatus(vehicle, ParkingStatus.ACTIVE);
            if (activeTransaction != null)
            {
                _logger.LogWarning("Vehicle '{LicensePlate}' already has an active parking session.", request.LicensePlate);
                
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", $"Vehicle '{request.LicensePlate}' already has an active parking session.");
            }

            // 4. Parking Space Allocation.
            LegalPark.Models.Entities.ParkingSpot parkingSpot;
            if (!string.IsNullOrEmpty(request.SpotNumber))
            {
                var specificSpot = await _parkingSpotRepository.findBySpotNumberAndMerchant(request.SpotNumber, merchant);
                if (specificSpot == null)
                {
                    _logger.LogWarning("Parking spot '{SpotNumber}' not found at merchant '{MerchantCode}'.", request.SpotNumber, request.MerchantCode);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Parking spot '{request.SpotNumber}' not found at merchant '{request.MerchantCode}'.");
                }
                parkingSpot = specificSpot;

                if (parkingSpot.Status != ParkingSpotStatus.AVAILABLE)
                {
                    _logger.LogWarning("Parking spot '{SpotNumber}' is not available. Current status: {Status}", parkingSpot.SpotNumber, parkingSpot.Status);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", $"Parking spot '{parkingSpot.SpotNumber}' is not available. Current status: {parkingSpot.Status}.");
                }
            }
            else
            {
                var availableSpots = await _parkingSpotRepository.findByMerchantAndStatus(merchant, ParkingSpotStatus.AVAILABLE);
                if (availableSpots == null || !availableSpots.Any())
                {
                    _logger.LogWarning("No available parking spots at merchant '{MerchantCode}'.", request.MerchantCode);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.ServiceUnavailable, "FAILED", $"No available parking spots at merchant '{request.MerchantCode}'.");
                }
                parkingSpot = availableSpots.FirstOrDefault();
            }

            // 5. Update Parking Space Status.
            if (parkingSpot != null)
            {
                parkingSpot.Status = ParkingSpotStatus.OCCUPIED;
                _parkingSpotRepository.Update(parkingSpot);
                await _parkingSpotRepository.SaveChangesAsync();
                _logger.LogInformation("Parking spot '{SpotNumber}' status updated to OCCUPIED.", parkingSpot.SpotNumber);
            }
            else
            {
                _logger.LogError("An error occurred during parking spot allocation. Parking spot object is null.");
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", "An internal error occurred during spot allocation.");
            }

            // 6. Create a New Parking Transaction.
            var newTransaction = new LegalPark.Models.Entities.ParkingTransaction
            {
                Id = Guid.NewGuid(),
                Vehicle = vehicle,
                VehicleId = vehicle.Id,
                ParkingSpot = parkingSpot,
                ParkingSpotId = parkingSpot.Id,
                EntryTime = DateTime.UtcNow,
                Status = ParkingStatus.ACTIVE,
                PaymentStatus = PaymentStatus.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            await _parkingTransactionRepository.AddAsync(newTransaction);
            await _parkingTransactionRepository.SaveChangesAsync();

            _logger.LogInformation("New parking transaction created for Transaction ID: {TransactionId}", newTransaction.Id);

            // 7. Prepare your response and send it.
            var response = new ParkingTransactionResponse
            {
                Id = newTransaction.Id.ToString(),
                //Vehicle = newTransaction.Vehicle,
                Vehicle = new VehicleResponse
                {
                    Id = vehicle.Id.ToString(),
                    LicensePlate = vehicle.LicensePlate,
                    Type = vehicle.Type.ToString(),
                    Owner = new UserBasicResponse
                    {
                        Id = vehicle.Owner.Id.ToString(),
                        Username = vehicle.Owner.AccountName,
                        Email = vehicle.Owner.Email
                    }
                },
                //ParkingSpot = newTransaction.ParkingSpot,
                ParkingSpot = new ParkingSpotResponse
                {
                    Id = parkingSpot.Id.ToString(),
                    SpotNumber = parkingSpot.SpotNumber,
                    SpotType = parkingSpot.SpotType.ToString(),
                    Status = parkingSpot.Status.ToString(),
                    Floor = parkingSpot.Floor.GetValueOrDefault(),
                    Merchant = new MerchantResponse
                    {
                        Id = merchant.Id.ToString(),
                        MerchantName = merchant.MerchantName,
                        MerchantAddress = merchant.MerchantAddress
                    }
                },
                EntryTime = newTransaction.EntryTime,
                Status = newTransaction.Status.ToString(),
                PaymentStatus = newTransaction.PaymentStatus.ToString()

            };

            _logger.LogInformation("Successfully recorded parking entry for {LicensePlate}.", newTransaction.Vehicle.LicensePlate);

            
            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Parking entry recorded successfully.", response);
        }



        public async Task<IActionResult> RecordParkingExit(ParkingExitRequest request)
        {
            _logger.LogInformation("Processing parking exit request for license plate: {LicensePlate}", request.LicensePlate);

            // 1. Search for Vehicles
            var vehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not found.", request.LicensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{request.LicensePlate}' not registered.");
            }

            // 2. Find a Merchant
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);
            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found with code: {MerchantCode}", request.MerchantCode);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {request.MerchantCode}.");
            }

            // 3. Search for active transactions for this vehicle at the same merchant
            var activeTransaction = await _parkingTransactionRepository.findByVehicleAndStatus(vehicle, ParkingStatus.ACTIVE);
            if (activeTransaction == null)
            {
                _logger.LogWarning("No active parking session found for vehicle '{LicensePlate}'.", request.LicensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"No active parking session found for vehicle '{request.LicensePlate}'.");
            }

            // Verify that the active transaction is indeed at the same merchant.
            if (activeTransaction.ParkingSpot?.Merchant?.MerchantCode != request.MerchantCode)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Active parking session for this vehicle is not at the specified merchant.");
            }

            // 4. Update Exit Time & Calculate Parking Fees
            activeTransaction.ExitTime = DateTime.UtcNow;
            decimal totalCost = CalculateParkingCost(activeTransaction);
            activeTransaction.TotalCost = totalCost;

            // 5. Payment Process
            if (vehicle.Owner == null || vehicle.Owner.Id == Guid.Empty)
            {
                _logger.LogError("Vehicle owner information missing for payment. Cannot process payment.");
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", "Vehicle owner information missing for payment. Cannot process payment.");
            }

            PaymentResult paymentResult = await _paymentService.ProcessParkingPayment(
                vehicle.Owner.Id.ToString(),
                totalCost,
                activeTransaction.Id.ToString(),
                request.VerificationCode
            );

            // 6. Based on the payment results, update the transaction status.
            if (paymentResult == PaymentResult.SUCCESS)
            {
                activeTransaction.PaymentStatus = PaymentStatus.PAID;
                activeTransaction.Status = ParkingStatus.COMPLETED;

                // Update Parking Space Status
                var parkingSpot = activeTransaction.ParkingSpot;
                if (parkingSpot != null)
                {
                    parkingSpot.Status = ParkingSpotStatus.AVAILABLE;
                    _parkingSpotRepository.Update(parkingSpot);
                }

                // Save updated transactions and parking slots
                _parkingTransactionRepository.Update(activeTransaction);
                await _parkingTransactionRepository.SaveChangesAsync();

                // Prepare and send email notifications
                try
                {
                    var templateVariables = new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["name"] = vehicle.Owner.AccountName,
                        ["licensePlate"] = activeTransaction.Vehicle?.LicensePlate,
                        ["merchantName"] = activeTransaction.ParkingSpot?.Merchant?.MerchantName,
                        ["entryTime"] = activeTransaction.EntryTime.ToString("dd-MM-yyyy HH:mm"),
                        ["exitTime"] = activeTransaction.ExitTime?.ToString("dd-MM-yyyy HH:mm"),
                        ["totalCost"] = $"Rp {totalCost.ToString("N0")}"
                    };

                    string emailBody = await _templateService.ProcessEmailTemplateAsync("payment_success_confirmation", templateVariables);

                    var emailRequest = new LegalPark.Models.DTOs.Request.Notification.EmailNotificationRequest
                    {
                        To = vehicle.Owner.Email,
                        Subject = "LegalPark - Konfirmasi Pembayaran Parkir Berhasil!",
                        Body = emailBody
                    };
                    await _notificationService.SendEmailNotification(emailRequest);
                    _logger.LogInformation("Payment success confirmation email sent to user {UserId}: {UserEmail}", vehicle.Owner.Id, vehicle.Owner.Email);
                }
                catch (System.Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send payment confirmation email to user {UserId}: {UserEmail}", vehicle.Owner.Id, vehicle.Owner.Email);
                }

                var response = _parkingTransactionResponseMapper.MapToParkingTransactionResponse(activeTransaction);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Parking exit recorded successfully. Payment successful.", response);
            }
            else if (paymentResult == PaymentResult.INSUFFICIENT_BALANCE)
            {
                activeTransaction.PaymentStatus = PaymentStatus.FAILED;
                _parkingTransactionRepository.Update(activeTransaction);
                await _parkingTransactionRepository.SaveChangesAsync();
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Insufficient balance. Total cost: Rp {totalCost.ToString("N0")}.");
            }
            else
            {
                activeTransaction.PaymentStatus = PaymentStatus.FAILED;
                _parkingTransactionRepository.Update(activeTransaction);
                await _parkingTransactionRepository.SaveChangesAsync();
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", "Payment failed. Please check your verification code and try again.");
            }
        }




        
        private decimal CalculateParkingCost(LegalPark.Models.Entities.ParkingTransaction transaction)
        {
            if (transaction.EntryTime == null || transaction.ExitTime == null)
            {
                return 0;
            }

            // Calculate the duration in minutes.
            var duration = transaction.ExitTime.Value - transaction.EntryTime;
            long totalMinutes = (long)duration.TotalMinutes;

            // Example of a simple rate: Rp 5,000 per hour, rounded up.
            decimal hourlyRate = 5000;
            long hours = (totalMinutes + 59) / 60;
            if (hours == 0 && totalMinutes > 0)
            {
                hours = 1;
            }
            else if (totalMinutes == 0)
            {
                hours = 0;
            }

            return hourlyRate * hours;
        }

        public async Task<IActionResult> GetUserActiveParkingTransaction(string licensePlate)
        {
            // Search for vehicles
            var vehicle = await _vehicleRepository.findByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not registered.", licensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{licensePlate}' not registered.");
            }

            // Search for active transactions
            var activeTransaction = await _parkingTransactionRepository.findByVehicleAndStatus(vehicle, ParkingStatus.ACTIVE);
            if (activeTransaction == null)
            {
                
                _logger.LogInformation("No active parking session found for vehicle '{LicensePlate}'.", licensePlate);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "No active parking session found for vehicle '" + licensePlate + "'.", null);
            }

            
            var response = _parkingTransactionResponseMapper.MapToParkingTransactionResponse(activeTransaction);
            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", response);
        }

        
        public async Task<IActionResult> GetUserParkingTransactionHistory(string licensePlate)
        {
            
            var vehicle = await _vehicleRepository.findByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not registered.", licensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{licensePlate}' not registered.");
            }

            
            var transactions = await _parkingTransactionRepository.findByVehicle(vehicle);

            
            var responses = transactions.Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t)).ToList();

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", responses);
        }

        
        public async Task<IActionResult> GetUserParkingTransactionDetails(string transactionId, string licensePlate)
        {
            
            if (!Guid.TryParse(transactionId, out Guid transactionGuid))
            {
                _logger.LogWarning("Invalid transaction ID format: {TransactionId}", transactionId);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid transaction ID format.");
            }

            
            var transaction = await _parkingTransactionRepository.GetByIdWithDetailsAsync(transactionGuid);
            if (transaction == null)
            {
                _logger.LogWarning("Parking transaction not found with ID: {TransactionId}", transactionId);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Parking transaction not found with ID: {transactionId}.");
            }

            
            if (!transaction.Vehicle.LicensePlate.Equals(licensePlate))
            {
                _logger.LogWarning("Access denied. Transaction {TransactionId} does not belong to vehicle '{LicensePlate}'.", transactionId, licensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "Access denied. This transaction does not belong to the specified vehicle.");
            }

            
            var response = _parkingTransactionResponseMapper.MapToParkingTransactionResponse(transaction);
            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", response);
        }

    }
}



    



































