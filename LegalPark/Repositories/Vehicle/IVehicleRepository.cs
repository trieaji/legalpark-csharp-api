using LegalPark.Repositories.Generic;

namespace LegalPark.Repositories.Vehicle
{
    // Interface spesifik untuk VehicleRepository
    // Mewarisi dari IGenericRepository untuk mendapatkan metode dasar CRUD
    public interface IVehicleRepository : IGenericRepository<LegalPark.Models.Entities.Vehicle>
    {
        // Mencari kendaraan berdasarkan nomor plat
        // Mengembalikan Optional<Vehicle> di Java, di C# kita gunakan T? (nullable reference type)
        Task<LegalPark.Models.Entities.Vehicle?> findByLicensePlate(string licensePlate);

        // Mencari semua kendaraan berdasarkan pemilik (User)
        Task<List<LegalPark.Models.Entities.Vehicle>> findByOwner(LegalPark.Models.Entities.User owner);

        // Mencari kendaraan berdasarkan ID pemilik
        Task<LegalPark.Models.Entities.Vehicle?> findByOwnerId(Guid ownerId); // Menggunakan Guid untuk ownerId

        Task<List<LegalPark.Models.Entities.Vehicle>> GetAllWithDetailsAsync();

        Task<LegalPark.Models.Entities.Vehicle?> GetByIdWithDetailsAsync(Guid id);
    }
}
