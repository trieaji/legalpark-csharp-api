using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Notification
{
    public class NotificationService : INotificationService
    {
        // --- Deklarasi Field di Tingkat Kelas ---
        private readonly ILogger<NotificationService> _logger;
        private readonly MailService _mailService;

        /// <summary>
        /// Konstruktor untuk dependency injection ILogger dan MailService.
        /// </summary>
        /// <param name="logger">Instance logger untuk mencatat informasi.</param>
        /// <param name="mailService">Instance layanan email.</param>
        public NotificationService(ILogger<NotificationService> logger, MailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        /// <summary>
        /// Mengirim notifikasi email secara asinkron.
        /// </summary>
        /// <param name="request">Objek permintaan yang berisi detail email.</param>
        /// <returns>Hasil aksi yang menunjukkan status pengiriman.</returns>
        public async Task<IActionResult> SendEmailNotification(EmailNotificationRequest request)
        {
            try
            {
                // Memanggil layanan pengiriman email secara asinkron
                await _mailService.SendEmailAsync(request.To, request.Subject, request.Body);

                // Log informasi sukses
                _logger.LogInformation("Email notification sent successfully to: {ToEmail}", request.To);

                // Mengembalikan respons sukses menggunakan ResponseHandler
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Email notification sent successfully.", null);
            }
            // Mengubah 'Exception e' menjadi 'System.Exception e' untuk menghindari konflik nama
            catch (System.Exception e)
            {
                // Log kesalahan secara detail
                _logger.LogError(e, "Failed to send email notification to {ToEmail}: {ErrorMessage}", request.To, e.Message);

                // Mengembalikan respons error menggunakan ResponseHandler
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "Email sending failed: " + e.Message, "FAILED");
            }
        }
    }

}
