using System.Collections.Generic;
using System.Linq.Expressions; // Tambahkan ini!
using System.Threading.Tasks;

namespace LegalPark.Repositories.Generic
{
    // Interface generik untuk operasi CRUD dasar
    // T adalah tipe entitas (misalnya, Merchant, User, Vehicle)
    public interface IGenericRepository<T> where T : class
    {
        // Mendapatkan entitas berdasarkan Id
        Task<T?> GetByIdAsync(Guid id); // Menggunakan T? untuk Optional di Java

        // Mendapatkan semua entitas
        Task<IEnumerable<T>> GetAllAsync();

        // Menambahkan entitas baru
        Task AddAsync(T entity);

        // Memperbarui entitas yang sudah ada
        void Update(T entity);

        // Menghapus entitas
        void Delete(T entity);

        // Menyimpan perubahan ke database
        Task SaveChangesAsync();

        // Mendapatkan entitas berdasarkan kondisi tertentu (misalnya, Where)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    }
}
