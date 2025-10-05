using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.User
{
    
    public interface IUserRepository : IGenericRepository<LegalPark.Models.Entities.User>
    {
        
        Task<LegalPark.Models.Entities.User?> findByEmail(string email);

        
        Task<LegalPark.Models.Entities.User?> findByAccountName(string accountName);
    }
}
