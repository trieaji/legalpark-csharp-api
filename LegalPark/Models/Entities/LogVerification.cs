using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("log_verifications")]
    public class LogVerification
    {
        [Key]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code")]
        public string Code { get; set; }

        [Required]
        [Column("expired")]
        public DateTime Expired { get; set; }

        [Required]
        [Column("is_verify")]
        public bool IsVerify { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public LogVerification() { }

        public LogVerification(Guid id, Guid userId, string code, DateTime expired, bool isVerify, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Code = code;
            Expired = expired;
            IsVerify = isVerify;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public LogVerification(Guid userId, string code, DateTime expired, bool isVerify, DateTime createdAt, DateTime updatedAt)
        {
            UserId = userId;
            Code = code;
            Expired = expired;
            IsVerify = isVerify;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
