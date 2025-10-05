using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("users")]
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(50)]
        [Column("account_name")]
        public string AccountName { get; set; }

        [Required]
        [Column("role")]
        public Role Role { get; set; }

        [Required]
        [Column("account_status")]
        public AccountStatus AccountStatus { get; set; }

        [Required]
        [Column("balance", TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        
        public ICollection<Vehicle> Vehicles { get; set; }

        
        public ICollection<LogVerification> LogVerifications { get; set; }

        
        public ICollection<PaymentVerificationCode> PaymentVerificationCodes { get; set; }


        public User() : base()
        {
            
            Vehicles = new HashSet<Vehicle>();
            LogVerifications = new HashSet<LogVerification>();
            PaymentVerificationCodes = new HashSet<PaymentVerificationCode>();
        }

        
        public User(Guid id, string accountName, string email, string phoneNumber, Role role, AccountStatus accountStatus, decimal balance, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            AccountName = accountName;
            Email = email; 
            PhoneNumber = phoneNumber; 
            Role = role;
            AccountStatus = accountStatus;
            Balance = balance;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;

            Vehicles = new HashSet<Vehicle>();
            LogVerifications = new HashSet<LogVerification>();
            PaymentVerificationCodes = new HashSet<PaymentVerificationCode>();
        }

        
        public User(string accountName, string email, string phoneNumber, Role role, AccountStatus accountStatus, decimal balance, DateTime createdAt, DateTime updatedAt)
        {
            AccountName = accountName;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
            AccountStatus = accountStatus;
            Balance = balance;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;

            Vehicles = new HashSet<Vehicle>();
            LogVerifications = new HashSet<LogVerification>();
            PaymentVerificationCodes = new HashSet<PaymentVerificationCode>();
        }
    }

    public enum Role
    {
        USER,
        ADMIN,
        SUPERADMIN
    }

    public enum AccountStatus
    {
        PENDING_VERIFICATION,
        ACTIVE,
        INACTIVE,
        SUSPENDED,
        BLOCKED
    }
}
