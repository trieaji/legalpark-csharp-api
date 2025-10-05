using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.Vehicle
{
    
    public interface IVehicleRepository : IGenericRepository<LegalPark.Models.Entities.Vehicle>
    {
        
        Task<LegalPark.Models.Entities.Vehicle?> findByLicensePlate(string licensePlate);

        
        Task<List<LegalPark.Models.Entities.Vehicle>> findByOwner(LegalPark.Models.Entities.User owner);

        
        Task<LegalPark.Models.Entities.Vehicle?> findByOwnerId(Guid ownerId); 

        Task<List<LegalPark.Models.Entities.Vehicle>> GetAllWithDetailsAsync();

        Task<LegalPark.Models.Entities.Vehicle?> GetByIdWithDetailsAsync(Guid id);
    }
}
