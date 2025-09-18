using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalPark.Models.Entities
{
    [Table("merchants")]
    public class Merchant
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(8)]
        [Column("merchant_code")]
        public string MerchantCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("merchant_name")]
        public string MerchantName { get; set; }

        [MaxLength(255)]
        [Column("merchant_address")]
        public string MerchantAddress { get; set; }

        [MaxLength(100)]
        [Column("contact_person")]
        public string ContactPerson { get; set; }

        [MaxLength(20)]
        [Column("contact_phone")]
        public string ContactPhone { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // --- Properti Navigasi Koleksi ---
        // Satu Merchant bisa memiliki banyak ParkingSpot
        public ICollection<ParkingSpot> ParkingSpots { get; set; }

        public Merchant()
        {
            // Inisialisasi koleksi
            ParkingSpots = new HashSet<ParkingSpot>();
        }

        public Merchant(Guid id, string merchantCode, string merchantName, string merchantAddress,
                        string contactPerson, string contactPhone, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            MerchantCode = merchantCode;
            MerchantName = merchantName;
            MerchantAddress = merchantAddress;
            ContactPerson = contactPerson;
            ContactPhone = contactPhone;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ParkingSpots = new HashSet<ParkingSpot>();
        }

        public Merchant(string merchantCode, string merchantName, string merchantAddress,
                        string contactPerson, string contactPhone, DateTime createdAt, DateTime updatedAt)
        {
            MerchantCode = merchantCode;
            MerchantName = merchantName;
            MerchantAddress = merchantAddress;
            ContactPerson = contactPerson;
            ContactPhone = contactPhone;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ParkingSpots = new HashSet<ParkingSpot>();
        }
    }
}
