using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.ParkingSpot
{
    
    public interface IParkingSpotRepository : IGenericRepository<LegalPark.Models.Entities.ParkingSpot>
    {
        
        Task<LegalPark.Models.Entities.ParkingSpot?> findBySpotNumberAndMerchant(string spotNumber, LegalPark.Models.Entities.Merchant merchant);

        
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant(LegalPark.Models.Entities.Merchant merchant);

        
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatus(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status);

        
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatusAndSpotType(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status, LegalPark.Models.Entities.SpotType spotType);

        
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByFloorAndMerchant(int? floor, LegalPark.Models.Entities.Merchant merchant);

        
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant_Id(Guid merchantId);

        
        Task<List<LegalPark.Models.Entities.ParkingSpot>> findByStatus(LegalPark.Models.Entities.ParkingSpotStatus status);
    }
}
