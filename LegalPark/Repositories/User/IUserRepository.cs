using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.User
{
    // Interface spesifik untuk UserRepository
    // Mewarisi dari IGenericRepository untuk mendapatkan metode dasar CRUD
    public interface IUserRepository : IGenericRepository<LegalPark.Models.Entities.User>
    {
        // Mencari user berdasarkan email
        // Mengembalikan Optional<Users> di Java, di C# kita gunakan T? (nullable reference type)
        Task<LegalPark.Models.Entities.User?> findByEmail(string email);

        // Mencari user berdasarkan accountName
        Task<LegalPark.Models.Entities.User?> findByAccountName(string accountName);
    }
}
