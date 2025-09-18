using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.ParkingTransaction
{
    // Interface spesifik untuk ParkingTransactionRepository
    public interface IParkingTransactionRepository : IGenericRepository<LegalPark.Models.Entities.ParkingTransaction>
    {
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> GetAllWithDetailsAsync();

        Task<LegalPark.Models.Entities.ParkingTransaction?> GetByIdWithDetailsAsync(Guid id);

        Task<LegalPark.Models.Entities.ParkingTransaction?> UpdatePaymentStatusAsync(Guid transactionId, LegalPark.Models.Entities.PaymentStatus newPaymentStatus);


        // Mencari transaksi parkir berdasarkan status
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByStatus(LegalPark.Models.Entities.ParkingStatus status);

        // Mencari transaksi parkir berdasarkan status pembayaran
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByPaymentStatus(LegalPark.Models.Entities.PaymentStatus paymentStatus);

        // Mencari transaksi berdasarkan kendaraan
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicle(LegalPark.Models.Entities.Vehicle vehicle);

        // Mencari transaksi berdasarkan slot parkir
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByParkingSpot(LegalPark.Models.Entities.ParkingSpot parkingSpot);

        // Mencari transaksi aktif untuk kendaraan tertentu
        Task<LegalPark.Models.Entities.ParkingTransaction?> findByVehicleAndStatus(LegalPark.Models.Entities.Vehicle vehicle, LegalPark.Models.Entities.ParkingStatus status);

        // Mencari transaksi berdasarkan ID kendaraan
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicle_Id(Guid vehicleId);

        // Mencari transaksi berdasarkan merchant
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByParkingSpot_Merchant(LegalPark.Models.Entities.Merchant merchant);

        // Query: Mencari transaksi berdasarkan ID pemilik kendaraan (user) dan rentang waktu masuk
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicleOwnerIdAndEntryTimeBetween(Guid userId, DateTime startDateTime, DateTime endDateTime);

        // Query: Mencari semua transaksi parkir yang terkait dengan kendaraan milik user tertentu
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicleOwnerId(Guid userId);

        // Query: Mencari transaksi pembayaran yang berstatus 'PAID' dalam rentang waktu keluar tertentu
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findPaidTransactionsByExitTimeBetween(DateTime startOfDay, DateTime endOfDay);

        // Query: Mencari transaksi pembayaran yang berstatus 'PAID' dalam rentang waktu keluar tertentu dan di merchant spesifik
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findPaidTransactionsByExitTimeBetweenAndMerchantCode(DateTime startOfDay, DateTime endOfDay, string merchantCode);

        // Query: Mencari transaksi parkir aktif yang sedang menempati slot parkir tertentu
        Task<LegalPark.Models.Entities.ParkingTransaction?> findByParkingSpotAndStatus(LegalPark.Models.Entities.ParkingSpot parkingSpot, LegalPark.Models.Entities.ParkingStatus status);
    }
}
