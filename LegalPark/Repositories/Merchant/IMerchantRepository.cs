using LegalPark.Models.Entities; // Menggunakan entitas Merchant
using LegalPark.Repositories.Generic; // Tambahkan ini untuk IGenericRepository
using System.Threading.Tasks;

namespace LegalPark.Repositories.Merchant
{
    // Interface spesifik untuk MerchantRepository
    // Mewarisi dari IGenericRepository untuk mendapatkan metode dasar CRUD
    public interface IMerchantRepository : IGenericRepository<LegalPark.Models.Entities.Merchant>
    {
        // Metode kustom Anda: findByMerchantCode
        // Mengembalikan Optional<Merchant> di Java, di C# kita gunakan T? (nullable reference type)
        Task<LegalPark.Models.Entities.Merchant> FindByMerchantCodeAsync(string merchantCode);
    }
}
