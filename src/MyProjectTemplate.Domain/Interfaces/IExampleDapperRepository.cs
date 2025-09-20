using System.Linq.Expressions;

using Dodo.Primitives;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Entities;

namespace MyProjectTemplate.Domain.Interfaces;

public interface IExampleDapperRepository
{
    Task<Example?> GetByIdAsync(Uuid id);
    Task<(IEnumerable<Example> Items, int TotalCount)> 
        GetAllAsync(PaginationParams pagination,
                    Expression<Func<Example, bool>>? filter = null,
                    Func<IQueryable<Example>, IOrderedQueryable<Example>>? orderBy = null,
                    CancellationToken cancellationToken = default);
    Task<Example> AddAsync(Example entity);
    Task UpdateAsync(Example entity);
    Task DeleteAsync(Uuid id);
    Task<bool> ExistsAsync(Uuid id);
}
