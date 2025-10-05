using LegalPark.Data; 
using LegalPark.Models.Entities; 
using LegalPark.Repositories.Generic; 
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks;

namespace LegalPark.Repositories.Merchant
{
    
    public class MerchantRepository : GenericRepository<LegalPark.Models.Entities.Merchant>, IMerchantRepository 
    {
        public MerchantRepository(LegalParkDbContext context) : base(context)
        {
            
        }

        
        public async Task<LegalPark.Models.Entities.Merchant?> FindByMerchantCodeAsync(string merchantCode) 
        {
            
            return await _context.Merchants.FirstOrDefaultAsync(m => m.MerchantCode == merchantCode);
        }
    }
}
