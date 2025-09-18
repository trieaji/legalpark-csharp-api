using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.ParkingSpot
{
    // Implementasi spesifik untuk ParkingSpotRepository
    // Mewarisi dari GenericRepository untuk mendapatkan implementasi metode dasar CRUD
    public class ParkingSpotRepository : GenericRepository<LegalPark.Models.Entities.ParkingSpot>, IParkingSpotRepository
    {
        public ParkingSpotRepository(LegalParkDbContext context) : base(context)
        {
            // Constructor ini memanggil constructor kelas dasar (GenericRepository)
            // untuk menginisialisasi _context.
        }

        // Implementasi metode kustom findBySpotNumberAndMerchant
        public async Task<LegalPark.Models.Entities.ParkingSpot?> findBySpotNumberAndMerchant(string spotNumber, LegalPark.Models.Entities.Merchant merchant)
        {
            // Mencari slot parkir berdasarkan nomor slot dan objek merchant
            return await _context.ParkingSpots
                                 .Where(ps => ps.SpotNumber == spotNumber && ps.MerchantId == merchant.Id)
                                 .FirstOrDefaultAsync();
        }

        // Implementasi metode kustom findByMerchant
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant(LegalPark.Models.Entities.Merchant merchant)
        {
            // Mencari semua slot parkir berdasarkan objek merchant
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchant.Id)
                                 .ToListAsync();
        }

        // Implementasi metode kustom findByMerchantAndStatus
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatus(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status)
        {
            // Mencari slot parkir yang tersedia di merchant tertentu berdasarkan status
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchant.Id && ps.Status == status)
                                 .ToListAsync();
        }

        // Implementasi metode kustom findByMerchantAndStatusAndSpotType
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchantAndStatusAndSpotType(LegalPark.Models.Entities.Merchant merchant, LegalPark.Models.Entities.ParkingSpotStatus status, LegalPark.Models.Entities.SpotType spotType)
        {
            // Mencari slot parkir yang tersedia di merchant tertentu berdasarkan status dan tipe spot
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchant.Id && ps.Status == status && ps.SpotType == spotType)
                                 .ToListAsync();
        }

        // Implementasi metode kustom findByFloorAndMerchant
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByFloorAndMerchant(int? floor, LegalPark.Models.Entities.Merchant merchant)
        {
            // Mencari slot berdasarkan floor dan merchant
            return await _context.ParkingSpots
                                 .Where(ps => ps.Floor == floor && ps.MerchantId == merchant.Id)
                                 .ToListAsync();
        }

        // Implementasi metode kustom findByMerchant_Id
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByMerchant_Id(Guid merchantId)
        {
            // Mencari slot berdasarkan ID merchant saja
            return await _context.ParkingSpots
                                 .Where(ps => ps.MerchantId == merchantId)
                                 .ToListAsync();
        }

        // Implementasi metode kustom findByStatus
        public async Task<List<LegalPark.Models.Entities.ParkingSpot>> findByStatus(LegalPark.Models.Entities.ParkingSpotStatus status)
        {
            // Mencari semua slot parkir berdasarkan status
            return await _context.ParkingSpots
                                 .Where(ps => ps.Status == status)
                                 .ToListAsync();
        }
    }
}
