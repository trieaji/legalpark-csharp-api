using LegalPark.Models.DTOs.Request.Balance;
using LegalPark.Models.DTOs.Request.VerificationCode;
using LegalPark.Models.Entities;
using LegalPark.Repositories.ParkingTransaction;
using LegalPark.Services.Balance;
using LegalPark.Services.VerificationCode;
using LegalPark.Util;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        // Dependency Injection - bidang readonly untuk menyimpan instance layanan
        private readonly ILogger<PaymentService> _logger;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IBalanceService _balanceService;
        private readonly IParkingTransactionRepository _parkingTransactionRepository;

        // Konstruktor untuk menerima dependensi melalui injeksi
        public PaymentService(
            ILogger<PaymentService> logger,
            IVerificationCodeService verificationCodeService,
            IBalanceService balanceService,
            IParkingTransactionRepository parkingTransactionRepository)
        {
            _logger = logger;
            _verificationCodeService = verificationCodeService;
            _balanceService = balanceService;
            _parkingTransactionRepository = parkingTransactionRepository;
        }

        /// <summary>
        /// Memproses pembayaran simulasi untuk transaksi parkir.
        /// Mengikuti alur bisnis dari implementasi Java asli.
        /// </summary>
        /// <param name="userId">ID pengguna sebagai string.</param>
        /// <param name="amount">Jumlah yang harus dibayarkan.</param>
        /// <param name="parkingTransactionId">ID transaksi parkir sebagai string.</param>
        /// <param name="verificationCode">Kode verifikasi yang diberikan pengguna.</param>
        /// <returns>Hasil pembayaran dalam bentuk enum PaymentResult.</returns>
        public async Task<PaymentResult> ProcessParkingPayment(string userId, decimal amount, string parkingTransactionId, string verificationCode)
        {
            _logger.LogInformation("Processing payment for User ID: {UserId}, Amount: {Amount}, Parking Transaction ID: {ParkingTransactionId}",
                userId, amount, parkingTransactionId);

            try
            {
                // =========================================================
                // 1. Validasi Kode Verifikasi Pembayaran
                // =========================================================
                var verifyCodeRequest = new VerifyPaymentCodeRequest
                {
                    UserId = userId,
                    Code = verificationCode,
                    ParkingTransactionId = parkingTransactionId
                };

                // Panggil VerificationCodeService untuk validasi.
                var verificationResponse = await _verificationCodeService.ValidatePaymentVerificationCode(verifyCodeRequest);

                // Periksa status respons. Jika tidak berhasil (misalnya BadRequest), gagal.
                if (verificationResponse is ObjectResult vrObj)
                {
                    if (vrObj.StatusCode == (int)HttpStatusCode.OK || vrObj.StatusCode == null)
                    {
                        _logger.LogInformation("Verification code validated successfully for User ID: {UserId}", userId);
                    }
                    else
                    {
                        _logger.LogWarning("Payment FAILED: Verification code returned status {StatusCode} for User ID: {UserId}",
                            vrObj.StatusCode, userId);
                        return PaymentResult.FAILED_OTHER;
                    }
                }
                else if (verificationResponse is OkResult || verificationResponse is OkObjectResult)
                {
                    _logger.LogInformation("Verification code validated successfully for User ID: {UserId}", userId);
                }
                else
                {
                    _logger.LogWarning("Payment FAILED: Unknown verification response type {Type} for User ID: {UserId}",
                        verificationResponse.GetType().Name, userId);
                    return PaymentResult.FAILED_OTHER;
                }


                // =========================================================
                // 2. Memanggil BalanceService untuk mengurangi saldo
                // =========================================================
                var deductRequest = new DeductBalanceRequest
                {
                    UserId = userId,
                    Amount = amount
                };

                // Panggil layanan untuk mengurangi saldo
                var deductResponse = await _balanceService.DeductBalance(deductRequest);

                // Periksa status respons pengurangan saldo
                if (deductResponse is ObjectResult obj)
                {
                    if (obj.StatusCode == (int)HttpStatusCode.OK || obj.StatusCode == null)
                    {
                        // =========================================================
                        // 3. Update Status Transaksi Parkir
                        // =========================================================
                        if (!Guid.TryParse(parkingTransactionId, out var transactionGuid))
                        {
                            _logger.LogError("Invalid parking transaction ID format: {ParkingTransactionId}", parkingTransactionId);
                            return PaymentResult.FAILED_OTHER;
                        }

                        var parkingTransaction = await _parkingTransactionRepository.GetByIdAsync(transactionGuid);

                        if (parkingTransaction == null)
                        {
                            _logger.LogError("Parking Transaction {ParkingTransactionId} not found after payment.", parkingTransactionId);
                            return PaymentResult.FAILED_OTHER;
                        }

                        parkingTransaction.PaymentStatus = PaymentStatus.PAID;
                        parkingTransaction.UpdatedAt = DateTime.UtcNow;

                        _parkingTransactionRepository.Update(parkingTransaction);
                        await _parkingTransactionRepository.SaveChangesAsync();

                        _logger.LogInformation("Payment SUCCESS for User ID: {UserId}. Parking Transaction {ParkingTransactionId} updated to PAID.",
                            userId, parkingTransactionId);
                        return PaymentResult.SUCCESS;
                    }
                    else if (obj is BadRequestObjectResult badRequest &&
                             badRequest.Value?.ToString()?.Contains("Insufficient balance") == true)
                    {
                        _logger.LogWarning("Payment FAILED: Insufficient Balance for User ID: {UserId}", userId);
                        return PaymentResult.INSUFFICIENT_BALANCE;
                    }
                    else
                    {
                        _logger.LogWarning("Payment FAILED: DeductBalance returned status {StatusCode} for User ID: {UserId}",
                            obj.StatusCode, userId);
                        return PaymentResult.FAILED_OTHER;
                    }
                }
                else if (deductResponse is OkResult || deductResponse is OkObjectResult)
                {
                    // fallback: dianggap sukses
                    return PaymentResult.SUCCESS;
                }
                else
                {
                    _logger.LogWarning("Payment FAILED: Unknown response type {Type} from DeductBalance for User ID: {UserId}",
                        deductResponse.GetType().Name, userId);
                    return PaymentResult.FAILED_OTHER;
                }
            }
            catch (System.Exception ex)
            {
                // Tangani pengecualian tak terduga
                _logger.LogError(ex, "An error occurred during payment processing for User ID: {UserId}", userId);
                return PaymentResult.FAILED_OTHER;
            }
        }
    }
}
