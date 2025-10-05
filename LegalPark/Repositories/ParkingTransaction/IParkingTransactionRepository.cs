using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.ParkingTransaction
{
    
    public interface IParkingTransactionRepository : IGenericRepository<LegalPark.Models.Entities.ParkingTransaction>
    {
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> GetAllWithDetailsAsync();

        Task<LegalPark.Models.Entities.ParkingTransaction?> GetByIdWithDetailsAsync(Guid id);

        Task<LegalPark.Models.Entities.ParkingTransaction?> UpdatePaymentStatusAsync(Guid transactionId, LegalPark.Models.Entities.PaymentStatus newPaymentStatus);


        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByStatus(LegalPark.Models.Entities.ParkingStatus status);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByPaymentStatus(LegalPark.Models.Entities.PaymentStatus paymentStatus);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicle(LegalPark.Models.Entities.Vehicle vehicle);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByParkingSpot(LegalPark.Models.Entities.ParkingSpot parkingSpot);

        
        Task<LegalPark.Models.Entities.ParkingTransaction?> findByVehicleAndStatus(LegalPark.Models.Entities.Vehicle vehicle, LegalPark.Models.Entities.ParkingStatus status);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicle_Id(Guid vehicleId);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByParkingSpot_Merchant(LegalPark.Models.Entities.Merchant merchant);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicleOwnerIdAndEntryTimeBetween(Guid userId, DateTime startDateTime, DateTime endDateTime);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicleOwnerId(Guid userId);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findPaidTransactionsByExitTimeBetween(DateTime startOfDay, DateTime endOfDay);

        
        Task<List<LegalPark.Models.Entities.ParkingTransaction>> findPaidTransactionsByExitTimeBetweenAndMerchantCode(DateTime startOfDay, DateTime endOfDay, string merchantCode);

        
        Task<LegalPark.Models.Entities.ParkingTransaction?> findByParkingSpotAndStatus(LegalPark.Models.Entities.ParkingSpot parkingSpot, LegalPark.Models.Entities.ParkingStatus status);
    }
}
