using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.LogVerification
{
    // Interface spesifik untuk LogVerificationRepository
    // Mewarisi dari IGenericRepository untuk mendapatkan metode dasar CRUD
    public interface ILogVerificationRepository : IGenericRepository<LegalPark.Models.Entities.LogVerification>
    {
        // Metode kustom: getByUserAndExp (sesuai permintaan)
        // @Query("SELECT l FROM LogVerification l WHERE l.user.email = :email AND l.code = :otp AND l.isVerify = false")
        Task<LegalPark.Models.Entities.LogVerification?> getByUserAndExp(string email, string otp);

        // Metode kustom: findByUserEmail (sesuai permintaan)
        Task<LegalPark.Models.Entities.LogVerification?> findByUserEmail(string email);
    }
}
