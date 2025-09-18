using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using LegalPark.Models.DTOs.Request.VerificationCode;
using LegalPark.Models.Entities;
using LegalPark.Repositories.ParkingTransaction;
using LegalPark.Repositories.PaymentVerificationCode;
using LegalPark.Repositories.User;
using LegalPark.Services.Notification;
using LegalPark.Services.Template;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.VerificationCode
{
    public class VerificationCodeService : IVerificationCodeService
    {
        // Deklarasi field untuk Dependency Injection
        private readonly ILogger<VerificationCodeService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IParkingTransactionRepository _parkingTransactionRepository;
        private readonly IPaymentVerificationCodeRepository _paymentVerificationCodeRepository;
        private readonly INotificationService _notificationService;
        private readonly ITemplateService _templateService;

        // Konstruktor untuk inisialisasi melalui Dependency Injection
        public VerificationCodeService(
            ILogger<VerificationCodeService> logger,
            IUserRepository userRepository,
            IParkingTransactionRepository parkingTransactionRepository,
            IPaymentVerificationCodeRepository paymentVerificationCodeRepository,
            INotificationService notificationService,
            ITemplateService templateService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _parkingTransactionRepository = parkingTransactionRepository;
            _paymentVerificationCodeRepository = paymentVerificationCodeRepository;
            _notificationService = notificationService;
            _templateService = templateService;
        }

        // Metode helper untuk memeriksa apakah kode verifikasi sudah kedaluwarsa
        private bool IsCodeExpired(DateTime expiryDate, DateTime currentTime)
        {
            // C# DateTime.Compare mengembalikan nilai > 0 jika tanggal pertama lebih besar,
            // jadi isAfter() di Java setara dengan currentTime.CompareTo(expiryDate) > 0.
            return currentTime.CompareTo(expiryDate) > 0;
        }

        /// <summary>
        /// Menghasilkan dan mengirim kode verifikasi pembayaran ke pengguna.
        /// </summary>
        /// <param name="request">Objek permintaan yang berisi ID pengguna dan ID transaksi parkir.</param>
        /// <returns>IActionResult yang menunjukkan status operasi.</returns>
        public async Task<IActionResult> GenerateAndSendPaymentVerificationCode(PaymentVerificationCodeRequest request)
        {
            try
            {
                // 1. Cari pengguna berdasarkan ID
                var user = await _userRepository.GetByIdAsync(new Guid(request.UserId));
                if (user == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                }

                // 2. Cari transaksi parkir yang aktif berdasarkan ID transaksi
                var parkingTransaction = await _parkingTransactionRepository.GetByIdAsync(new Guid(request.ParkingTransactionId));
                if (parkingTransaction == null || parkingTransaction.Status != ParkingStatus.ACTIVE)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                        $"Active parking transaction not found or invalid for ID: {request.ParkingTransactionId}");
                }

                // 3. Hasilkan kode OTP dan tanggal kedaluwarsa
                string otpCode = GenerateOtp.GenerateRandomNumber();
                DateTime expiryDate = GenerateOtp.GetExpiryDate();

                // 4. Buat objek PaymentVerificationCode baru
                var paymentCode = new PaymentVerificationCode
                {
                    UserId = user.Id,
                    Code = otpCode,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = expiryDate,
                    IsVerified = false,
                    ParkingTransactionId = parkingTransaction.Id,
                };

                // Catatan: Karena properti navigasi `User` dan `ParkingTransaction` tidak dapat
                // diset langsung karena konflik relasi, kita cukup set foreign key-nya (UserId dan ParkingTransactionId).
                // EF Core akan mengurus sisanya.

                // 5. Simpan kode verifikasi ke database
                await _paymentVerificationCodeRepository.AddAsync(paymentCode);
                await _paymentVerificationCodeRepository.SaveChangesAsync();

                // 6. Siapkan variabel untuk template email
                var templateVariables = new Dictionary<string, object>
                {
                    ["name"] = user.AccountName,
                    ["otp"] = otpCode
                };

                // Perlu sedikit penyesuaian karena RazorLight menggunakan objek anonim atau class
                // bukan Dictionary seperti di Java.
                string emailBody = await _templateService.ProcessEmailTemplateAsync("payment_otp_verification", templateVariables);

                // 7. Buat permintaan notifikasi email
                var emailRequest = new EmailNotificationRequest
                {
                    To = user.Email,
                    Subject = "LegalPark - Kode Verifikasi Pembayaran Anda",
                    Body = emailBody
                };

                // 8. Kirim email
                await _notificationService.SendEmailNotification(emailRequest);

                _logger.LogInformation("Payment verification code generated and sent to user {UserId}: {UserEmail}", user.Id, user.Email);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Payment verification code sent.", null);

            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error generating and sending payment verification code for user {UserId}: {ErrorMessage}", request.UserId, e.Message);
                // Melempar exception untuk memastikan transaksi tidak tersimpan,
                // mirip dengan perilaku @Transactional di Java saat terjadi RuntimeException
                throw new System.Exception($"Failed to generate and send payment verification code: {e.Message}", e);
            }
        }

        /// <summary>
        /// Memvalidasi kode verifikasi pembayaran yang diberikan pengguna.
        /// </summary>
        /// <param name="request">Objek permintaan yang berisi ID pengguna, kode, dan ID transaksi.</param>
        /// <returns>IActionResult yang menunjukkan status validasi.</returns>
        public async Task<IActionResult> ValidatePaymentVerificationCode(VerifyPaymentCodeRequest request)
        {
            try
            {
                // 1. Validasi format GUID untuk UserId
                if (!Guid.TryParse(request.UserId, out var userGuid))
                {
                    return ResponseHandler.GenerateResponseError(
                        HttpStatusCode.BadRequest,
                        "FAILED",
                        "Invalid User ID format."
                    );
                }

                // 2. Validasi format GUID untuk ParkingTransactionId
                if (!Guid.TryParse(request.ParkingTransactionId, out var transactionGuid))
                {
                    return ResponseHandler.GenerateResponseError(
                        HttpStatusCode.BadRequest,
                        "FAILED",
                        "Invalid Parking Transaction ID format."
                    );
                }

                // 1. Cari pengguna berdasarkan ID
                var user = await _userRepository.GetByIdAsync(new Guid(request.UserId));
                if (user == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                }

                // 2. Cari kode verifikasi terbaru yang belum diverifikasi
                var verification = await _paymentVerificationCodeRepository
                    .findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc(user, request.Code);

                if (verification == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid or expired verification code.");
                }

                // 3. Periksa apakah kode sudah kedaluwarsa
                if (IsCodeExpired(verification.ExpiresAt, DateTime.Now))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Verification code has expired.");
                }

                // 4. Periksa apakah kode verifikasi terhubung ke transaksi yang benar
                //if (verification.ParkingTransactionId == null || !verification.ParkingTransactionId.Equals(new Guid(request.ParkingTransactionId)))
                //{
                //    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Verification code is not valid for this transaction.");
                //}

                if (verification.ParkingTransactionId == null || verification.ParkingTransactionId != transactionGuid)
                {
                    return ResponseHandler.GenerateResponseError(
                        HttpStatusCode.BadRequest,
                        "FAILED",
                        "Verification code is not valid for this transaction."
                    );
                }

                // 5. Tandai kode verifikasi sebagai diverifikasi
                verification.IsVerified = true;
                _paymentVerificationCodeRepository.Update(verification);
                await _paymentVerificationCodeRepository.SaveChangesAsync();

                _logger.LogInformation("Payment verification code for user {UserId} validated successfully for transaction {TransactionId}.", user.Id, request.ParkingTransactionId);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Payment verification successful.", null);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error validating payment verification code for user {UserId}: {ErrorMessage}", request.UserId, e.Message);
                // Melempar exception untuk memastikan transaksi tidak tersimpan
                throw new System.Exception($"Failed to validate payment verification code: {e.Message}", e);
            }
        }
    }
}
