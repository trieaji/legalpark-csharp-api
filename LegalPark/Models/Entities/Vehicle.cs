using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("vehicles")]
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("license_plate")]
        public string LicensePlate { get; set; }

        [Required]
        [Column("type")]
        public VehicleType Type { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }

        public User Owner { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // --- Properti Navigasi Koleksi ---
        // Satu Vehicle bisa memiliki banyak ParkingTransaction
        public ICollection<ParkingTransaction> ParkingTransactions { get; set; }

        public Vehicle()
        {
            // Inisialisasi koleksi
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }

        public Vehicle(Guid id, string licensePlate, VehicleType type, Guid ownerId, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            LicensePlate = licensePlate;
            Type = type;
            OwnerId = ownerId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }

        public Vehicle(string licensePlate, VehicleType type, Guid ownerId, DateTime createdAt, DateTime updatedAt)
        {
            LicensePlate = licensePlate;
            Type = type;
            OwnerId = ownerId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }
    }

    public enum VehicleType
    {
        MOTORCYCLE,
        CAR,
        TRUCK,
        OTHER
    }
}
