using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("parking_spots")]
    public class ParkingSpot
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("spot_number")]
        public string SpotNumber { get; set; }

        [Required]
        [Column("spot_type")]
        public SpotType SpotType { get; set; }

        [Required]
        [Column("status")]
        public ParkingSpotStatus Status { get; set; }

        [Column("floor")]
        public int? Floor { get; set; }

        [Column("merchant_id")]
        public Guid MerchantId { get; set; }

        public Merchant Merchant { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // --- Properti Navigasi Koleksi ---
        // Satu ParkingSpot bisa memiliki banyak ParkingTransaction
        public ICollection<ParkingTransaction> ParkingTransactions { get; set; }

        public ParkingSpot()
        {
            // Inisialisasi koleksi
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }

        public ParkingSpot(Guid id, string spotNumber, SpotType spotType, ParkingSpotStatus status, int? floor, Guid merchantId, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            SpotNumber = spotNumber;
            SpotType = spotType;
            Status = status;
            Floor = floor;
            MerchantId = merchantId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }

        public ParkingSpot(string spotNumber, SpotType spotType, ParkingSpotStatus status, int? floor, Guid merchantId, DateTime createdAt, DateTime updatedAt)
        {
            SpotNumber = spotNumber;
            SpotType = spotType;
            Status = status;
            Floor = floor;
            MerchantId = merchantId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }
    }

    public enum SpotType
    {
        CAR, // -> 0
        MOTORCYCLE, // -> 1
        BICYCLE // -> 2
    }

    public enum ParkingSpotStatus
    {
        AVAILABLE, // -> 0
        OCCUPIED, // -> 1
        MAINTENANCE, // -> 2
        RESERVED // -> 3
    }
}
