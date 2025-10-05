using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("payment_verification_codes")]
    public class PaymentVerificationCode
    {
        [Key]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [Required]
        [Column("code")]
        public string Code { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Required]
        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Column("parking_transaction_id")]
        public Guid? ParkingTransactionId { get; set; }

        
        public ParkingTransaction ParkingTransaction { get; set; }

        public PaymentVerificationCode() { }

        public PaymentVerificationCode(Guid id, Guid userId, string code, DateTime createdAt, DateTime expiresAt, bool isVerified, Guid? parkingTransactionId)
        {
            Id = id;
            UserId = userId;
            Code = code;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsVerified = isVerified;
            ParkingTransactionId = parkingTransactionId;
        }

        public PaymentVerificationCode(Guid userId, string code, DateTime createdAt, DateTime expiresAt, bool isVerified, Guid? parkingTransactionId)
        {
            UserId = userId;
            Code = code;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsVerified = isVerified;
            ParkingTransactionId = parkingTransactionId;
        }

        public PaymentVerificationCode(User user, string code, DateTime createdAt, DateTime expiresAt)
        {
            User = user;
            UserId = user.Id;
            Code = code;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsVerified = false;
        }
    }
}
