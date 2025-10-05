using LegalPark.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LegalPark.Repositories.Generic 
{
    
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LegalParkDbContext _context; 

        public GenericRepository(LegalParkDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            
            return await _context.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            
            _context.Set<T>().Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            
            return await _context.Set<T>().Where(expression).ToListAsync();
        }
    }
}