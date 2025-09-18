using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LegalPark.Helpers
{
    public class MailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<MailService> _logger;

        /// <summary>
        /// Konstruktor untuk inisialisasi MailService dengan dependency injection.
        /// </summary>
        /// <param name="smtpSettings">Pengaturan SMTP dari konfigurasi.</param>
        /// <param name="logger">Logger untuk mencatat informasi.</param>
        public MailService(IOptions<SmtpSettings> smtpSettings, ILogger<MailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Mengirim email secara asinkron dengan 3 kali percobaan jika gagal.
        /// </summary>
        /// <param name="toEmail">Alamat email penerima.</param>
        /// <param name="subject">Subjek email.</param>
        /// <param name="body">Isi email dalam format HTML.</param>
        /// <returns>Task yang merepresentasikan operasi pengiriman email.</returns>
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            int attempts = 0;
            bool sent = false;

            // Logika retry, maksimal 3 kali percobaan
            while (attempts < 3 && !sent)
            {
                try
                {
                    // Membuat objek MailMessage
                    var message = new MailMessage();
                    message.From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true; // Menandakan body email adalah HTML

                    // Membuat objek SmtpClient
                    using (var client = new SmtpClient(_smtpSettings.SmtpHost, _smtpSettings.SmtpPort))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpSettings.SmtpUser, _smtpSettings.SmtpPass);

                        await client.SendMailAsync(message);
                    }

                    sent = true;
                    _logger.LogInformation("Email berhasil dikirim ke {ToEmail}", toEmail);
                }
                catch (SmtpException ex)
                {
                    attempts++;
                    _logger.LogError(ex, "Percobaan ke-{Attempt} gagal mengirim email. Error: {ErrorMessage}", attempts, ex.Message);

                    if (attempts == 3)
                    {
                        _logger.LogError("Semua percobaan mengirim email ke {ToEmail} gagal.", toEmail);
                    }
                }
            }
        }
    }
}
