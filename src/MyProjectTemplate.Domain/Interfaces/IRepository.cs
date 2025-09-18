using Dodo.Primitives;

namespace MyProjectTemplate.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Uuid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Uuid id);
    Task<bool> ExistsAsync(Uuid id);
}