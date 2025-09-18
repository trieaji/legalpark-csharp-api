using LegalPark.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LegalPark.Repositories.Generic // Namespace diubah
{
    // Implementasi generik dari IGenericRepository
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LegalParkDbContext _context; // DbContext untuk interaksi database

        public GenericRepository(LegalParkDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            // Mencari entitas berdasarkan Id.
            // FindAsync adalah metode yang efisien untuk mencari berdasarkan primary key.
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Mengambil semua entitas dari tabel.
            return await _context.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            // Menambahkan entitas baru ke DbContext.
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            // Menandai entitas sebagai 'Modified' agar EF Core tahu perlu diupdate.
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            // Menandai entitas untuk dihapus.
            _context.Set<T>().Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            // Menyimpan semua perubahan yang tertunda di DbContext ke database.
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            // Mencari entitas berdasarkan ekspresi LINQ (mirip dengan kriteria WHERE).
            return await _context.Set<T>().Where(expression).ToListAsync();
        }
    }
}