using LegalPark.Models.Entities; 
using LegalPark.Repositories.Generic; 
using System.Threading.Tasks;

namespace LegalPark.Repositories.Merchant
{
    
    public interface IMerchantRepository : IGenericRepository<LegalPark.Models.Entities.Merchant>
    {
        
        Task<LegalPark.Models.Entities.Merchant> FindByMerchantCodeAsync(string merchantCode);
    }
}
