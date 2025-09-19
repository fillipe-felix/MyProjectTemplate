using System.Linq.Expressions;

using Dodo.Primitives;

using MyProjectTemplate.Core.Base;

namespace MyProjectTemplate.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Uuid id);
    Task<(IEnumerable<T> Items, int TotalCount)> 
        GetAllAsync(PaginationParams pagination,
                      Expression<Func<T, bool>>? filter = null,
                      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                      CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Uuid id);
    Task<bool> ExistsAsync(Uuid id);
}