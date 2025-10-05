using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.LogVerification
{
    
    public class LogVerificationRepository : GenericRepository<LegalPark.Models.Entities.LogVerification>, ILogVerificationRepository
    {
        public LogVerificationRepository(LegalParkDbContext context) : base(context)
        {
            
        }

        
        public async Task<LegalPark.Models.Entities.LogVerification?> getByUserAndExp(string email, string otp)
        {
            
            return await _context.LogVerifications
                                 .Include(lv => lv.User) 
                                 .Where(lv => lv.User.Email == email && lv.Code == otp && lv.IsVerify == false)
                                 .FirstOrDefaultAsync();
        }

        
        public async Task<LegalPark.Models.Entities.LogVerification?> findByUserEmail(string email)
        {
            
            return await _context.LogVerifications
                                 .Include(lv => lv.User) 
                                 .FirstOrDefaultAsync(lv => lv.User.Email == email);
        }
    }
}
