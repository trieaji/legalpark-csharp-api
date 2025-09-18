using LegalPark.Models.DTOs.Request.VerificationCode;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.VerificationCode
{
    public interface IVerificationCodeService
    {
        Task<IActionResult> GenerateAndSendPaymentVerificationCode(PaymentVerificationCodeRequest request);
        Task<IActionResult> ValidatePaymentVerificationCode(VerifyPaymentCodeRequest request);
    }
}
