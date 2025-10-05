using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.LogVerification
{
    
    public interface ILogVerificationRepository : IGenericRepository<LegalPark.Models.Entities.LogVerification>
    {
        
        Task<LegalPark.Models.Entities.LogVerification?> getByUserAndExp(string email, string otp);

        
        Task<LegalPark.Models.Entities.LogVerification?> findByUserEmail(string email);
    }
}
