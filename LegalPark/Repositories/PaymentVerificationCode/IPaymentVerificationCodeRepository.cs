using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.PaymentVerificationCode
{
    
    public interface IPaymentVerificationCodeRepository : IGenericRepository<LegalPark.Models.Entities.PaymentVerificationCode>
    {
        
        Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc(LegalPark.Models.Entities.User user, string code);

        
        Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndIsVerifiedFalseAndExpiresAtAfterOrderByCreatedAtDesc(LegalPark.Models.Entities.User user, DateTime currentTime);
    }
}
