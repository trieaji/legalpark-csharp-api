using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.PaymentVerificationCode
{
    
    public class PaymentVerificationCodeRepository : GenericRepository<LegalPark.Models.Entities.PaymentVerificationCode>, IPaymentVerificationCodeRepository
    {
        public PaymentVerificationCodeRepository(LegalParkDbContext context) : base(context)
        {
            
        }

        
        public async Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc(LegalPark.Models.Entities.User user, string code)
        {
            
            return await _context.PaymentVerificationCodes
                                 .Include(pvc => pvc.User) 
                                 .Where(pvc => pvc.UserId == user.Id && pvc.Code == code && pvc.IsVerified == false)
                                 .OrderByDescending(pvc => pvc.ExpiresAt) 
                                 .FirstOrDefaultAsync(); 
        }

        
        public async Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndIsVerifiedFalseAndExpiresAtAfterOrderByCreatedAtDesc(LegalPark.Models.Entities.User user, DateTime currentTime)
        {
            
            return await _context.PaymentVerificationCodes
                                 .Include(pvc => pvc.User) 
                                 .Where(pvc => pvc.UserId == user.Id && pvc.IsVerified == false && pvc.ExpiresAt > currentTime)
                                 .OrderByDescending(pvc => pvc.CreatedAt) 
                                 .FirstOrDefaultAsync(); 
        }
    }
}
