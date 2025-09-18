using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.ParkingSpot
{
    // Interface spesifik untuk ParkingSpotRepository
    // Mewarisi dari IGenericRepository untuk mendapatkan metode dasar CRUD
    public interface IParkingSpotRepository : IGenericRepository<LegalPark.Models.Entities.ParkingSpot>
    {
        // Mencari slot parkir berdasarkan nomor slot dan merchant (untuk UniqueConstraint)
        // Mengembalikan Optional<ParkingSpot> di Java, di C# kita gunakan T? (nullable reference type)
        Task<LegalPark.Models.Entities.ParkingSpot?> findBySpotNumberAndMerchant(string spotNumber, LegalPark.Models.Entities.Merchant merchant);

        // Mencari semua slot parkir berdasarkan merchant
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant(LegalPark.Models.Entities.Merchant merchant);

        // Mencari slot parkir yang tersedia di merchant tertentu berdasarkan status AVAILABLE
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatus(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status);

        // Mencari slot parkir yang tersedia di merchant tertentu berdasarkan status dan tipe spot
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatusAndSpotType(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status, LegalPark.Models.Entities.SpotType spotType);

        // Opsional: Mencari slot berdasarkan floor dan merchant
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByFloorAndMerchant(int? floor, LegalPark.Models.Entities.Merchant merchant);

        // Opsional: Mencari slot berdasarkan ID merchant saja (tanpa perlu fetch objek Merchant)
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant_Id(Guid merchantId);

        // Opsional: Mencari slot berdasarkan status (misal: semua yang AVAILABLE di semua merchant)
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByStatus(LegalPark.Models.Entities.ParkingSpotStatus status);
    }
}
