using LegalPark.Models.DTOs.Request.Notification;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Notification
{
    public interface INotificationService
    {
        Task<IActionResult> SendEmailNotification(EmailNotificationRequest request);
    }
}
