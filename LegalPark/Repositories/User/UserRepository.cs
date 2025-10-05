using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.User
{
    
    public class UserRepository : GenericRepository<LegalPark.Models.Entities.User>, IUserRepository
    {
        public UserRepository(LegalParkDbContext context) : base(context)
        {
            
        }

        
        public async Task<LegalPark.Models.Entities.User?> findByEmail(string email)
        {
            
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        
        public async Task<LegalPark.Models.Entities.User?> findByAccountName(string accountName)
        {
            
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountName == accountName);
        }
    }
}
