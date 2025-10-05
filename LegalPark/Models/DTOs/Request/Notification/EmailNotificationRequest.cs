using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Notification
{
    public class EmailNotificationRequest
    {
        
        [Required(ErrorMessage = "Recipient email cannot be blank")]
        [EmailAddress(ErrorMessage = "Recipient email must be a valid email address")]
        public string To { get; set; }

        
        [Required(ErrorMessage = "Subject cannot be blank")]
        public string Subject { get; set; }

        
        [Required(ErrorMessage = "Body cannot be blank")]
        public string Body { get; set; }

        
        public string Cc { get; set; }

        
        public string Bcc { get; set; }
    }
}
