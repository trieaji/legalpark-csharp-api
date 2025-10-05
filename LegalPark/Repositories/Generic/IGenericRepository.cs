using System.Collections.Generic;
using System.Linq.Expressions; 
using System.Threading.Tasks;

namespace LegalPark.Repositories.Generic
{
    
    public interface IGenericRepository<T> where T : class
    {
        
        Task<T?> GetByIdAsync(Guid id); 

        
        Task<IEnumerable<T>> GetAllAsync();

        
        Task AddAsync(T entity);

        
        void Update(T entity);

        
        void Delete(T entity);

        
        Task SaveChangesAsync();

        
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    }
}
