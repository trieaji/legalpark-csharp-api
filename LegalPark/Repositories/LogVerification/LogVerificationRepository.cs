using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.LogVerification
{
    // Implementasi spesifik untuk LogVerificationRepository
    // Mewarisi dari GenericRepository untuk mendapatkan implementasi metode dasar CRUD
    public class LogVerificationRepository : GenericRepository<LegalPark.Models.Entities.LogVerification>, ILogVerificationRepository
    {
        public LogVerificationRepository(LegalParkDbContext context) : base(context)
        {
            // Constructor ini memanggil constructor kelas dasar (GenericRepository)
            // untuk menginisialisasi _context.
        }

        // Implementasi metode kustom getByUserAndExp (sesuai permintaan)
        public async Task<LegalPark.Models.Entities.LogVerification?> getByUserAndExp(string email, string otp)
        {
            // Menerjemahkan @Query JPQL ke LINQ
            return await _context.LogVerifications
                                 .Include(lv => lv.User) // Pastikan User dimuat jika diperlukan untuk filter email
                                 .Where(lv => lv.User.Email == email && lv.Code == otp && lv.IsVerify == false)
                                 .FirstOrDefaultAsync();
        }

        // Implementasi metode kustom findByUserEmail (sesuai permintaan)
        public async Task<LegalPark.Models.Entities.LogVerification?> findByUserEmail(string email)
        {
            // Mencari LogVerification berdasarkan email pengguna.
            return await _context.LogVerifications
                                 .Include(lv => lv.User) // Memuat objek User yang terkait
                                 .FirstOrDefaultAsync(lv => lv.User.Email == email);
        }
    }
}
