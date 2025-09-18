using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.PaymentVerificationCode
{
    // Interface spesifik untuk PaymentVerificationCodeRepository
    // Mewarisi dari IGenericRepository untuk mendapatkan metode dasar CRUD
    public interface IPaymentVerificationCodeRepository : IGenericRepository<LegalPark.Models.Entities.PaymentVerificationCode>
    {
        // Mencari kode verifikasi yang belum diverifikasi untuk user dan kode tertentu, diurutkan berdasarkan waktu kadaluarsa terbaru
        Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndCodeAndIsVerifiedFalseOrderByExpiresAtDesc(LegalPark.Models.Entities.User user, string code);

        // Mencari kode verifikasi aktif untuk user tertentu (jika hanya ada satu yang aktif pada satu waktu)
        Task<LegalPark.Models.Entities.PaymentVerificationCode?> findTopByUserAndIsVerifiedFalseAndExpiresAtAfterOrderByCreatedAtDesc(LegalPark.Models.Entities.User user, DateTime currentTime);
    }
}
