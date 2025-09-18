using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.User
{
    // Implementasi spesifik untuk UserRepository
    // Mewarisi dari GenericRepository untuk mendapatkan implementasi metode dasar CRUD
    public class UserRepository : GenericRepository<LegalPark.Models.Entities.User>, IUserRepository
    {
        public UserRepository(LegalParkDbContext context) : base(context)
        {
            // Constructor ini memanggil constructor kelas dasar (GenericRepository)
            // untuk menginisialisasi _context.
        }

        // Implementasi metode kustom findByEmail
        public async Task<LegalPark.Models.Entities.User?> findByEmail(string email)
        {
            // Mencari user berdasarkan email.
            // FirstOrDefaultAsync akan mengembalikan null jika tidak ditemukan.
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Implementasi metode kustom findByAccountName
        public async Task<LegalPark.Models.Entities.User?> findByAccountName(string accountName)
        {
            // Mencari user berdasarkan accountName.
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountName == accountName);
        }
    }
}
