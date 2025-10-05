using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.Entities;
using LegalPark.Repositories.LogVerification;
using LegalPark.Repositories.User;
using LegalPark.Services.Notification;
using LegalPark.Services.Template;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace LegalPark.Services.User
{
    public class UserService : IUserService
    {
        
        private readonly InfoAccount _infoAccount;
        private readonly IUserRepository _usersRepository;
        private readonly ILogVerificationRepository _logVerificationRepository;
        private readonly INotificationService _notificationService;
        private readonly ITemplateService _templateService;

        
        public UserService(
            InfoAccount infoAccount,
            IUserRepository usersRepository,
            ILogVerificationRepository logVerificationRepository,
            INotificationService notificationService,
            ITemplateService templateService)
        {
            _infoAccount = infoAccount;
            _usersRepository = usersRepository;
            _logVerificationRepository = logVerificationRepository;
            _notificationService = notificationService;
            _templateService = templateService;
        }

        
        public async Task<IActionResult> VerificationAccount(AccountVerification request)
        {
            try
            {
                
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var currentUser = await _infoAccount.GetAsync();
                    string userEmail = currentUser?.Email;

                    
                    var verify = await _logVerificationRepository.getByUserAndExp(userEmail, request.Code);

                    
                    if (verify == null)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Otp yang anda masukan salah");
                    }

                    // OTP expiration check logic
                    if (verify.Expired != null)
                    {
                        var getExpi = verify.Expired;
                        var currentTime = DateTime.Now;

                        if (IsExpired(getExpi, currentTime))
                        {
                            
                            var newVerification = new LogVerification
                            {
                                UserId = verify.UserId,
                                Code = GenerateOtp.GenerateRandomNumber(),
                                Expired = GenerateOtp.GetExpiryDate(),
                                IsVerify = false
                            };
                            await _logVerificationRepository.AddAsync(newVerification);
                            scope.Complete(); 
                            return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "OTP expired. A new OTP has been generated.");
                        }
                    }

                    
                    verify.IsVerify = true;
                    _logVerificationRepository.Update(verify);

                    await _logVerificationRepository.SaveChangesAsync();

                    // Retrieving user data and updating account status
                    var user = await _usersRepository.findByEmail(userEmail);
                    if (user == null)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                    }
                    user.AccountStatus = AccountStatus.ACTIVE;
                    _usersRepository.Update(user);

                    await _usersRepository.SaveChangesAsync();

                    // Sending a successful verification email notification
                    var templateVariables = new Dictionary<string, object>
                    {
                        { "name", user.AccountName }
                    };
                    string emailBody = await _templateService.ProcessEmailTemplateAsync("email_success_verification", templateVariables);

                    var emailRequest = new EmailNotificationRequest
                    {
                        To = user.Email,
                        Subject = "Success Verification",
                        Body = emailBody
                    };
                    await _notificationService.SendEmailNotification(emailRequest);

                    scope.Complete(); 

                    var responseDto = new UserVerificationResponse
                    {
                        Email = user.Email,
                        AccountName = user.AccountName,
                        AccountStatus = user.AccountStatus.ToString()
                    };

                    return ResponseHandler.GenerateResponseSuccess(responseDto);
                }
            }
            catch (System.Exception e)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, e.Message, "Internal Server Error");
            }
        }

        
        public async Task<IActionResult> UpdateAccountStatus(string userId, AccountStatus newStatus)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid user ID format.");
                }

                var user = await _usersRepository.GetByIdAsync(userGuid);
                if (user == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found with ID: " + userId);
                }

                
                if (user.AccountStatus == AccountStatus.BLOCKED && newStatus != AccountStatus.ACTIVE)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Cannot change status from BLOCKED directly, unless unblocked by admin.");
                }

                if (user.AccountStatus == AccountStatus.PENDING_VERIFICATION &&
                    !(newStatus == AccountStatus.ACTIVE || newStatus == AccountStatus.BLOCKED))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid status transition from PENDING_VERIFICATION.");
                }

                user.AccountStatus = newStatus;
                _usersRepository.Update(user);

                await _usersRepository.SaveChangesAsync();

                scope.Complete(); 
                return ResponseHandler.GenerateResponseSuccess(user);
            }
        }

        
        private static bool IsExpired(DateTime expiredTime, DateTime currentTime)
        {
            return (currentTime - expiredTime).TotalMinutes >= 5;
        }
    }
}
