using LegalPark.Models.DTOs.Request.VerificationCode;
using LegalPark.Services.VerificationCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.VerificationCode
{
    
    [ApiController]
    [Route("api/v1/payment/verification")]
    
    public class VerificationCodeController : ControllerBase
    {
        
        private readonly ILogger<VerificationCodeController> _logger;
        private readonly IVerificationCodeService _verificationCodeService;

        
        public VerificationCodeController(ILogger<VerificationCodeController> logger, IVerificationCodeService verificationCodeService)
        {
            _logger = logger;
            _verificationCodeService = verificationCodeService;
        }

        
        [Authorize(Roles = "User")]
        [HttpPost("generate")]
        public async Task<IActionResult> GeneratePaymentVerificationCode([FromBody] PaymentVerificationCodeRequest request)
        {
            _logger.LogInformation("Received request to generate payment verification code for userId: {UserId}", request.UserId);
            return await _verificationCodeService.GenerateAndSendPaymentVerificationCode(request);
        }

        
        [Authorize(Roles = "User")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePaymentVerificationCode([FromBody] VerifyPaymentCodeRequest request)
        {
            _logger.LogInformation("Received request to validate payment verification code for userId: {UserId}", request.UserId);
            return await _verificationCodeService.ValidatePaymentVerificationCode(request);
        }
    }

}
