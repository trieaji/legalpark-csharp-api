using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.Vehicle
{
    // Implementasi spesifik untuk VehicleRepository
    // Mewarisi dari GenericRepository untuk mendapatkan implementasi metode dasar CRUD
    public class VehicleRepository : GenericRepository<LegalPark.Models.Entities.Vehicle>, IVehicleRepository
    {
        public VehicleRepository(LegalParkDbContext context) : base(context)
        {
            // Constructor ini memanggil constructor kelas dasar (GenericRepository)
            // untuk menginisialisasi _context.
        }

        // Implementasi metode kustom findByLicensePlate
        public async Task<LegalPark.Models.Entities.Vehicle?> findByLicensePlate(string licensePlate)
        {
            // Mencari kendaraan berdasarkan nomor plat.
            return await _context.Vehicles.Include(v => v.Owner).FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
        
        }


        // Implementasi metode kustom findByOwner
        public async Task<List<LegalPark.Models.Entities.Vehicle>> findByOwner(LegalPark.Models.Entities.User owner)
        {
            // Mencari semua kendaraan berdasarkan objek pemilik (User).
            return await _context.Vehicles
                                 .Where(v => v.OwnerId == owner.Id)
                                 .ToListAsync();
        }
       

        // Implementasi metode kustom findByOwnerId
        public async Task<LegalPark.Models.Entities.Vehicle?> findByOwnerId(Guid ownerId)
        {
            // Mencari kendaraan berdasarkan ID pemilik.
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.OwnerId == ownerId);
        }


        public async Task<List<LegalPark.Models.Entities.Vehicle>> GetAllWithDetailsAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                 
                .ToListAsync();
        }

        public async Task<LegalPark.Models.Entities.Vehicle?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .Where(pt => pt.Id == id)
                .FirstOrDefaultAsync();
        }

    }
}
