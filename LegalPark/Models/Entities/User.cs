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

        // --- Properti Navigasi Koleksi ---
        // Satu User bisa memiliki banyak Vehicle
        public ICollection<Vehicle> Vehicles { get; set; }

        // Satu User bisa memiliki banyak LogVerification
        public ICollection<LogVerification> LogVerifications { get; set; }

        // Satu User bisa memiliki banyak PaymentVerificationCode
        public ICollection<PaymentVerificationCode> PaymentVerificationCodes { get; set; }


        public User() : base()
        {
            // Inisialisasi koleksi untuk menghindari NullReferenceException
            Vehicles = new HashSet<Vehicle>();
            LogVerifications = new HashSet<LogVerification>();
            PaymentVerificationCodes = new HashSet<PaymentVerificationCode>();
        }

        // Constructor dengan parameter lengkap (jika ingin digunakan)
        public User(Guid id, string accountName, string email, string phoneNumber, Role role, AccountStatus accountStatus, decimal balance, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            AccountName = accountName;
            Email = email; // Properti dari IdentityUser
            PhoneNumber = phoneNumber; // Properti dari IdentityUser
            Role = role;
            AccountStatus = accountStatus;
            Balance = balance;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;

            Vehicles = new HashSet<Vehicle>();
            LogVerifications = new HashSet<LogVerification>();
            PaymentVerificationCodes = new HashSet<PaymentVerificationCode>();
        }

        // Overload constructor tanpa Id
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
