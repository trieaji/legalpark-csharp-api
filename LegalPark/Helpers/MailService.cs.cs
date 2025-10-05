using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LegalPark.Helpers
{
    public class MailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<MailService> _logger;

        
        public MailService(IOptions<SmtpSettings> smtpSettings, ILogger<MailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            int attempts = 0;
            bool sent = false;

            
            while (attempts < 3 && !sent)
            {
                try
                {
                    
                    var message = new MailMessage();
                    message.From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true; 

                    
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
