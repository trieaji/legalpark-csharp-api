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
        
        private readonly ILogger<VerificationCodeService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IParkingTransactionRepository _parkingTransactionRepository;
        private readonly IPaymentVerificationCodeRepository _paymentVerificationCodeRepository;
        private readonly INotificationService _notificationService;
        private readonly ITemplateService _templateService;

        
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

        // Helper method to check if the verification code has expired
        private bool IsCodeExpired(DateTime expiryDate, DateTime currentTime)
        {
            
            return currentTime.CompareTo(expiryDate) > 0;
        }

        
        /// Generate and send payment verification codes to users.
        public async Task<IActionResult> GenerateAndSendPaymentVerificationCode(PaymentVerificationCodeRequest request)
        {
            try
            {
                // 1. Search for users by ID
                var user = await _userRepository.GetByIdAsync(new Guid(request.UserId));
                if (user == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                }

                // 2. Search for active parking transactions based on transaction ID
                var parkingTransaction = await _parkingTransactionRepository.GetByIdAsync(new Guid(request.ParkingTransactionId));
                if (parkingTransaction == null || parkingTransaction.Status != ParkingStatus.ACTIVE)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                        $"Active parking transaction not found or invalid for ID: {request.ParkingTransactionId}");
                }

                // 3. Generate OTP code and expiration date
                string otpCode = GenerateOtp.GenerateRandomNumber();
                DateTime expiryDate = GenerateOtp.GetExpiryDate();

                // 4. Create a new PaymentVerificationCode object
                var paymentCode = new PaymentVerificationCode
                {
                    UserId = user.Id,
                    Code = otpCode,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = expiryDate,
                    IsVerified = false,
                    ParkingTransactionId = parkingTransaction.Id,
                };



                // 5. Save the verification code to the database
                await _paymentVerificationCodeRepository.AddAsync(paymentCode);
                await _paymentVerificationCodeRepository.SaveChangesAsync();

                // 6. Prepare variables for the email template
                var templateVariables = new Dictionary<string, object>
                {
                    ["name"] = user.AccountName,
                    ["otp"] = otpCode
                };

                
                string emailBody = await _templateService.ProcessEmailTemplateAsync("payment_otp_verification", templateVariables);

                // 7. Create an email notification request
                var emailRequest = new EmailNotificationRequest
                {
                    To = user.Email,
                    Subject = "LegalPark - Kode Verifikasi Pembayaran Anda",
                    Body = emailBody
                };

                // 8. Send email
                await _notificationService.SendEmailNotification(emailRequest);

                _logger.LogInformation("Payment verification code generated and sent to user {UserId}: {UserEmail}", user.Id, user.Email);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Payment verification code sent.", null);

            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error generating and sending payment verification code for user {UserId}: {ErrorMessage}", request.UserId, e.Message);
                // Throw an exception to ensure the transaction is not saved
                throw new System.Exception($"Failed to generate and send payment verification code: {e.Message}", e);
            }
        }

        
        /// Validate the payment verification code provided by the user.
        public async Task<IActionResult> ValidatePaymentVerificationCode(VerifyPaymentCodeRequest request)
        {
            try
            {
                
                if (!Guid.TryParse(request.UserId, out var userGuid))
                {
                    return ResponseHandler.GenerateResponseError(
                        HttpStatusCode.BadRequest,
                        "FAILED",
                        "Invalid User ID format."
                    );
                }

                
                if (!Guid.TryParse(request.ParkingTransactionId, out var transactionGuid))
                {
                    return ResponseHandler.GenerateResponseError(
                        HttpStatusCode.BadRequest,
                        "FAILED",
                        "Invalid Parking Transaction ID format."
                    );
                }

                // 1. Search for users by ID
                var user = await _userRepository.GetByIdAsync(new Guid(request.UserId));
                if (user == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                }

                // 2. Find the latest unverified verification code
                var verification = await _paymentVerificationCodeRepository
                    .findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc(user, request.Code);

                if (verification == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid or expired verification code.");
                }

                // 3. Check if the code has expired
                if (IsCodeExpired(verification.ExpiresAt, DateTime.Now))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Verification code has expired.");
                }

                // 4. Check whether the verification code is linked to the correct transaction.
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

                // 5. Mark the verification code as verified
                verification.IsVerified = true;
                _paymentVerificationCodeRepository.Update(verification);
                await _paymentVerificationCodeRepository.SaveChangesAsync();

                _logger.LogInformation("Payment verification code for user {UserId} validated successfully for transaction {TransactionId}.", user.Id, request.ParkingTransactionId);
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Payment verification successful.", null);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error validating payment verification code for user {UserId}: {ErrorMessage}", request.UserId, e.Message);
                // Throw an exception to ensure the transaction is not saved
                throw new System.Exception($"Failed to validate payment verification code: {e.Message}", e);
            }
        }
    }
}
