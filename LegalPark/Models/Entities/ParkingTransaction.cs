using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("parking_transactions")]
    public class ParkingTransaction
    {
        [Key]
        public Guid Id { get; set; }

        [Column("vehicle_id")]
        public Guid VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }

        [Column("parking_spot_id")]
        public Guid? ParkingSpotId { get; set; }

        public ParkingSpot ParkingSpot { get; set; }

        [Required]
        [Column("entry_time")]
        public DateTime EntryTime { get; set; }

        [Column("exit_time")]
        public DateTime? ExitTime { get; set; }

        [Column("total_cost", TypeName = "decimal(10,2)")]
        public decimal? TotalCost { get; set; }

        [Required]
        [Column("status")]
        public ParkingStatus Status { get; set; }

        [Required]
        [Column("payment_status")]
        public PaymentStatus PaymentStatus { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        

        public ParkingTransaction() { }

        public ParkingTransaction(Guid id, Guid vehicleId, Guid? parkingSpotId, DateTime entryTime, DateTime? exitTime, decimal? totalCost, ParkingStatus status, PaymentStatus paymentStatus, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            VehicleId = vehicleId;
            ParkingSpotId = parkingSpotId;
            EntryTime = entryTime;
            ExitTime = exitTime;
            TotalCost = totalCost;
            Status = status;
            PaymentStatus = paymentStatus;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public ParkingTransaction(Guid vehicleId, Guid? parkingSpotId, DateTime entryTime, DateTime? exitTime, decimal? totalCost, ParkingStatus status, PaymentStatus paymentStatus, DateTime createdAt, DateTime updatedAt)
        {
            VehicleId = vehicleId;
            ParkingSpotId = parkingSpotId;
            EntryTime = entryTime;
            ExitTime = exitTime;
            TotalCost = totalCost;
            Status = status;
            PaymentStatus = paymentStatus;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public enum ParkingStatus
    {
        ACTIVE,
        COMPLETED,
        CANCELLED
    }

    public enum PaymentStatus
    {
        PENDING,
        PAID,
        FAILED
    }
}
