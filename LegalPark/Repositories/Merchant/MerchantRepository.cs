using LegalPark.Data; // Menggunakan LegalParkDbContext
using LegalPark.Models.Entities; // Menggunakan entitas Merchant
using LegalPark.Repositories.Generic; // Tambahkan ini untuk GenericRepository
using Microsoft.EntityFrameworkCore; // Untuk metode seperti FirstOrDefaultAsync
using System.Threading.Tasks;

namespace LegalPark.Repositories.Merchant
{
    // Implementasi spesifik untuk MerchantRepository
    // Mewarisi dari GenericRepository untuk mendapatkan implementasi metode dasar CRUD
    public class MerchantRepository : GenericRepository<LegalPark.Models.Entities.Merchant>, IMerchantRepository // Perbaikan di sini
    {
        public MerchantRepository(LegalParkDbContext context) : base(context)
        {
            // Constructor ini memanggil constructor kelas dasar (GenericRepository)
            // untuk menginisialisasi _context.
        }

        // Implementasi metode kustom findByMerchantCode
        public async Task<LegalPark.Models.Entities.Merchant?> FindByMerchantCodeAsync(string merchantCode) // Perbaikan di sini
        {
            // Mencari merchant berdasarkan merchantCode.
            // FirstOrDefaultAsync akan mengembalikan null jika tidak ditemukan,
            // yang setara dengan Optional.empty() di Java.
            return await _context.Merchants.FirstOrDefaultAsync(m => m.MerchantCode == merchantCode);
        }
    }
}
