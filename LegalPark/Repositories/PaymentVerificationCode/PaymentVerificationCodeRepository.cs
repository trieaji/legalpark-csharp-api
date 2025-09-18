using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.PaymentVerificationCode
{
    // Implementasi spesifik untuk PaymentVerificationCodeRepository
    // Mewarisi dari GenericRepository untuk mendapatkan implementasi metode dasar CRUD
    public class PaymentVerificationCodeRepository : GenericRepository<LegalPark.Models.Entities.PaymentVerificationCode>, IPaymentVerificationCodeRepository
    {
        public PaymentVerificationCodeRepository(LegalParkDbContext context) : base(context)
        {
            // Constructor ini memanggil constructor kelas dasar (GenericRepository)
            // untuk menginisialisasi _context.
        }

        // Implementasi metode kustom findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc
        public async Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc(LegalPark.Models.Entities.User user, string code)
        {
            // Mencari kode verifikasi yang belum diverifikasi untuk user dan kode tertentu, diurutkan berdasarkan waktu kadaluarsa terbaru
            return await _context.PaymentVerificationCodes
                                 .Include(pvc => pvc.User) // Memuat objek User yang terkait
                                 .Where(pvc => pvc.UserId == user.Id && pvc.Code == code && pvc.IsVerified == false)
                                 .OrderByDescending(pvc => pvc.ExpiresAt) // Order by expiresAt descending
                                 .FirstOrDefaultAsync(); // Mengambil yang paling atas
        }

        // Implementasi metode kustom findTopByUserAndIsVerifiedFalseAndExpiresAtAfterOrderByCreatedAtDesc
        public async Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndIsVerifiedFalseAndExpiresAtAfterOrderByCreatedAtDesc(LegalPark.Models.Entities.User user, DateTime currentTime)
        {
            // Mencari kode verifikasi aktif untuk user tertentu
            return await _context.PaymentVerificationCodes
                                 .Include(pvc => pvc.User) // Memuat objek User yang terkait
                                 .Where(pvc => pvc.UserId == user.Id && pvc.IsVerified == false && pvc.ExpiresAt > currentTime)
                                 .OrderByDescending(pvc => pvc.CreatedAt) // Order by createdAt descending
                                 .FirstOrDefaultAsync(); // Mengambil yang paling atas
        }
    }
}
