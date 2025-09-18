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
        // Field untuk dependency injection dari repository dan service.
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

        // Constructor untuk menginjeksikan dependensi.
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

        /// <summary>
        /// Merekam entri parkir baru untuk sebuah kendaraan.
        /// </summary>
        /// <param name="request">Objek request yang berisi detail entri parkir.</param>
        /// <returns>IActionResult yang mengindikasikan keberhasilan atau kegagalan.</returns>
        public async Task<IActionResult> RecordParkingEntry(ParkingEntryRequest request)
        {
            _logger.LogInformation("Processing parking entry request for license plate: {LicensePlate}", request.LicensePlate);

            // 1. Cari Kendaraan berdasarkan plat nomor.
            var vehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not found.", request.LicensePlate);
                // Menggunakan ResponseHandler.GenerateResponseError
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{request.LicensePlate}' not registered.");
            }

            // 2. Cari Merchant berdasarkan kode merchant.
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);
            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found with code: {MerchantCode}", request.MerchantCode);
                // Menggunakan ResponseHandler.GenerateResponseError
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {request.MerchantCode}.");
            }

            // 3. Cek apakah kendaraan sudah memiliki transaksi aktif.
            var activeTransaction = await _parkingTransactionRepository.findByVehicleAndStatus(vehicle, ParkingStatus.ACTIVE);
            if (activeTransaction != null)
            {
                _logger.LogWarning("Vehicle '{LicensePlate}' already has an active parking session.", request.LicensePlate);
                // Menggunakan ResponseHandler.GenerateResponseError
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", $"Vehicle '{request.LicensePlate}' already has an active parking session.");
            }

            // 4. Alokasi Slot Parkir.
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

            // 5. Update Status Slot Parkir.
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

            // 6. Buat Transaksi Parkir Baru.
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

            // 7. Siapkan response dan kirimkan.
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

            // Menggunakan GenerateResponseSuccess dengan status dan pesan
            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Parking entry recorded successfully.", response);
        }



        public async Task<IActionResult> RecordParkingExit(ParkingExitRequest request)
        {
            _logger.LogInformation("Processing parking exit request for license plate: {LicensePlate}", request.LicensePlate);

            // 1. Cari Kendaraan
            var vehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not found.", request.LicensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{request.LicensePlate}' not registered.");
            }

            // 2. Cari Merchant
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);
            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found with code: {MerchantCode}", request.MerchantCode);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {request.MerchantCode}.");
            }

            // 3. Cari transaksi aktif untuk kendaraan ini di merchant yang sama
            var activeTransaction = await _parkingTransactionRepository.findByVehicleAndStatus(vehicle, ParkingStatus.ACTIVE);
            if (activeTransaction == null)
            {
                _logger.LogWarning("No active parking session found for vehicle '{LicensePlate}'.", request.LicensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"No active parking session found for vehicle '{request.LicensePlate}'.");
            }

            // Verifikasi bahwa transaksi aktif memang di merchant yang sama
            if (activeTransaction.ParkingSpot?.Merchant?.MerchantCode != request.MerchantCode)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Active parking session for this vehicle is not at the specified merchant.");
            }

            // 4. Update Waktu Keluar & Hitung Biaya Parkir
            activeTransaction.ExitTime = DateTime.UtcNow;
            decimal totalCost = CalculateParkingCost(activeTransaction);
            activeTransaction.TotalCost = totalCost;

            // 5. Proses Pembayaran
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

            // 6. Berdasarkan hasil pembayaran, perbarui status transaksi
            if (paymentResult == PaymentResult.SUCCESS)
            {
                activeTransaction.PaymentStatus = PaymentStatus.PAID;
                activeTransaction.Status = ParkingStatus.COMPLETED;

                // Update Status Slot Parkir
                var parkingSpot = activeTransaction.ParkingSpot;
                if (parkingSpot != null)
                {
                    parkingSpot.Status = ParkingSpotStatus.AVAILABLE;
                    _parkingSpotRepository.Update(parkingSpot);
                }

                // Simpan transaksi dan slot parkir yang sudah diperbarui
                _parkingTransactionRepository.Update(activeTransaction);
                await _parkingTransactionRepository.SaveChangesAsync();

                // Siapkan dan kirim notifikasi email
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




        /// <summary>
        /// Menghitung total biaya parkir berdasarkan durasi.
        /// </summary>
        private decimal CalculateParkingCost(LegalPark.Models.Entities.ParkingTransaction transaction)
        {
            if (transaction.EntryTime == null || transaction.ExitTime == null)
            {
                return 0;
            }

            // Hitung durasi dalam menit.
            var duration = transaction.ExitTime.Value - transaction.EntryTime;
            long totalMinutes = (long)duration.TotalMinutes;

            // Contoh tarif sederhana: Rp 5.000 per jam, pembulatan ke atas.
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
            // Cari kendaraan
            var vehicle = await _vehicleRepository.findByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not registered.", licensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{licensePlate}' not registered.");
            }

            // Cari transaksi aktif
            var activeTransaction = await _parkingTransactionRepository.findByVehicleAndStatus(vehicle, ParkingStatus.ACTIVE);
            if (activeTransaction == null)
            {
                // Note: Berbeda dengan Java, respons ini mengembalikan 'OK' tapi dengan pesan "No active session".
                _logger.LogInformation("No active parking session found for vehicle '{LicensePlate}'.", licensePlate);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "No active parking session found for vehicle '" + licensePlate + "'.", null);
            }

            // Map entitas ke DTO dan kembalikan respons sukses
            var response = _parkingTransactionResponseMapper.MapToParkingTransactionResponse(activeTransaction);
            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", response);
        }

        /// <summary>
        /// Mendapatkan riwayat transaksi parkir untuk kendaraan.
        /// </summary>
        /// <param name="licensePlate">Plat nomor kendaraan.</param>
        public async Task<IActionResult> GetUserParkingTransactionHistory(string licensePlate)
        {
            // Cari kendaraan
            var vehicle = await _vehicleRepository.findByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with license plate '{LicensePlate}' not registered.", licensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with license plate '{licensePlate}' not registered.");
            }

            // Ambil semua transaksi
            var transactions = await _parkingTransactionRepository.findByVehicle(vehicle);

            // Menggunakan LINQ untuk memetakan daftar entitas ke daftar DTO
            var responses = transactions.Select(t => _parkingTransactionResponseMapper.MapToParkingTransactionResponse(t)).ToList();

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", responses);
        }

        /// <summary>
        /// Mendapatkan detail transaksi parkir tertentu.
        /// </summary>
        /// <param name="transactionId">ID transaksi.</param>
        /// <param name="licensePlate">Plat nomor kendaraan.</param>
        public async Task<IActionResult> GetUserParkingTransactionDetails(string transactionId, string licensePlate)
        {
            // Konversi string transactionId ke Guid.
            // Gunakan TryParse untuk penanganan error yang aman jika string tidak valid.
            if (!Guid.TryParse(transactionId, out Guid transactionGuid))
            {
                _logger.LogWarning("Invalid transaction ID format: {TransactionId}", transactionId);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid transaction ID format.");
            }

            // Cari transaksi berdasarkan ID
            var transaction = await _parkingTransactionRepository.GetByIdWithDetailsAsync(transactionGuid);
            if (transaction == null)
            {
                _logger.LogWarning("Parking transaction not found with ID: {TransactionId}", transactionId);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Parking transaction not found with ID: {transactionId}.");
            }

            // Validasi keamanan: Pastikan transaksi ini milik kendaraan yang memiliki plat nomor yang diberikan
            if (!transaction.Vehicle.LicensePlate.Equals(licensePlate))
            {
                _logger.LogWarning("Access denied. Transaction {TransactionId} does not belong to vehicle '{LicensePlate}'.", transactionId, licensePlate);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "Access denied. This transaction does not belong to the specified vehicle.");
            }

            // Map entitas ke DTO dan kembalikan respons sukses
            var response = _parkingTransactionResponseMapper.MapToParkingTransactionResponse(transaction);
            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", response);
        }

    }
}



    



































