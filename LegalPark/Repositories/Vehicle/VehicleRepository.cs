using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.Vehicle
{
    
    public class VehicleRepository : GenericRepository<LegalPark.Models.Entities.Vehicle>, IVehicleRepository
    {
        public VehicleRepository(LegalParkDbContext context) : base(context)
        {
            
        }

        
        public async Task<LegalPark.Models.Entities.Vehicle?> findByLicensePlate(string licensePlate)
        {
            
            return await _context.Vehicles.Include(v => v.Owner).FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
        
        }


        
        public async Task<List<LegalPark.Models.Entities.Vehicle>> findByOwner(LegalPark.Models.Entities.User owner)
        {
            
            return await _context.Vehicles
                                 .Where(v => v.OwnerId == owner.Id)
                                 .ToListAsync();
        }
       

        
        public async Task<LegalPark.Models.Entities.Vehicle?> findByOwnerId(Guid ownerId)
        {
            
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
