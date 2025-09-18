using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Notification
{
    public class EmailNotificationRequest
    {
        /// <summary>
        /// Alamat email penerima.
        /// </summary>
        [Required(ErrorMessage = "Recipient email cannot be blank")]
        [EmailAddress(ErrorMessage = "Recipient email must be a valid email address")]
        public string To { get; set; }

        /// <summary>
        /// Subjek email.
        /// </summary>
        [Required(ErrorMessage = "Subject cannot be blank")]
        public string Subject { get; set; }

        /// <summary>
        /// Isi pesan email.
        /// </summary>
        [Required(ErrorMessage = "Body cannot be blank")]
        public string Body { get; set; }

        /// <summary>
        /// Alamat email Carbon Copy (CC) (opsional).
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// Alamat email Blind Carbon Copy (BCC) (opsional).
        /// </summary>
        public string Bcc { get; set; }
    }
}
