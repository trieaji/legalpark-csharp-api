using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Notification
{
    public class NotificationService : INotificationService
    {
        
        private readonly ILogger<NotificationService> _logger;
        private readonly MailService _mailService;

        
        public NotificationService(ILogger<NotificationService> logger, MailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        
        public async Task<IActionResult> SendEmailNotification(EmailNotificationRequest request)
        {
            try
            {
                
                await _mailService.SendEmailAsync(request.To, request.Subject, request.Body);

                
                _logger.LogInformation("Email notification sent successfully to: {ToEmail}", request.To);

                
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Email notification sent successfully.", null);
            }
            
            catch (System.Exception e)
            {
                
                _logger.LogError(e, "Failed to send email notification to {ToEmail}: {ErrorMessage}", request.To, e.Message);

                
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "Email sending failed: " + e.Message, "FAILED");
            }
        }
    }

}
