using LegalPark.Models.DTOs.Request.VerificationCode;
using LegalPark.Services.VerificationCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.VerificationCode
{
    /// <summary>
    /// Controller untuk mengelola endpoint kode verifikasi.
    /// Memungkinkan klien untuk berinteraksi langsung dengan langkah-langkah yang melibatkan
    /// kode verifikasi (meminta dan memverifikasi).
    /// </summary>
    [ApiController]
    [Route("api/v1/payment/verification")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("Verification code API", Description = "Memungkinkan klien (aplikasi mobile/web) untuk secara langsung berinteraksi dengan langkah-langkah yang melibatkan kode verifikasi (meminta dan memverifikasi)")]
    public class VerificationCodeController : ControllerBase
    {
        // Menggunakan ILogger untuk logging, ini adalah pola standar di ASP.NET Core
        private readonly ILogger<VerificationCodeController> _logger;
        private readonly IVerificationCodeService _verificationCodeService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public VerificationCodeController(ILogger<VerificationCodeController> logger, IVerificationCodeService verificationCodeService)
        {
            _logger = logger;
            _verificationCodeService = verificationCodeService;
        }

        /// <summary>
        /// Endpoint untuk meminta (generate dan kirim) kode verifikasi pembayaran.
        /// Klien akan memanggil ini untuk memulai proses verifikasi pembayaran.
        /// </summary>
        /// <param name="request">Request body berisi ID pengguna untuk verifikasi.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("generate")]
        public async Task<IActionResult> GeneratePaymentVerificationCode([FromBody] PaymentVerificationCodeRequest request)
        {
            _logger.LogInformation("Received request to generate payment verification code for userId: {UserId}", request.UserId);
            return await _verificationCodeService.GenerateAndSendPaymentVerificationCode(request);
        }

        /// <summary>
        /// Endpoint untuk memvalidasi kode verifikasi pembayaran yang diterima pengguna.
        /// Klien akan memanggil ini setelah pengguna memasukkan kode OTP.
        /// </summary>
        /// <param name="request">Request body berisi ID pengguna dan kode verifikasi.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePaymentVerificationCode([FromBody] VerifyPaymentCodeRequest request)
        {
            _logger.LogInformation("Received request to validate payment verification code for userId: {UserId}", request.UserId);
            return await _verificationCodeService.ValidatePaymentVerificationCode(request);
        }
    }

}
