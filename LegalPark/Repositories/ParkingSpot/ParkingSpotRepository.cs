using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.ParkingSpot
{
    
    public class ParkingSpotRepository : GenericRepository<LegalPark.Models.Entities.ParkingSpot>, IParkingSpotRepository
    {
        public ParkingSpotRepository(LegalParkDbContext context) : base(context)
        {
            
        }

        
        public async Task<LegalPark.Models.Entities.ParkingSpot?> findBySpotNumberAndMerchant(string spotNumber, LegalPark.Models.Entities.Merchant merchant)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.SpotNumber == spotNumber && ps.MerchantId == merchant.Id)
                                 .FirstOrDefaultAsync();
        }

        
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant(LegalPark.Models.Entities.Merchant merchant)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchant.Id)
                                 .ToListAsync();
        }

        
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatus(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchant.Id && ps.Status == status)
                                 .ToListAsync();
        }

        
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatusAndSpotType(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status, LegalPark.Models.Entities.SpotType spotType)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchant.Id && ps.Status == status && ps.SpotType == spotType)
                                 .ToListAsync();
        }

        
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByFloorAndMerchant(int? floor, LegalPark.Models.Entities.Merchant merchant)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.Floor == floor && ps.MerchantId == merchant.Id)
                                 .ToListAsync();
        }

        
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant_Id(Guid merchantId)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchantId)
                                 .ToListAsync();
        }

        
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByStatus(LegalPark.Models.Entities.ParkingSpotStatus status)
        {
            
            return await _context.ParkingSpots
                                 .Where(ps => ps.Status == status)
                                 .ToListAsync();
        }
    }
}
