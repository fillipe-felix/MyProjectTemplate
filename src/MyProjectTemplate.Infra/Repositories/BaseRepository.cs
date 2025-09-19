using System.Linq.Expressions;

using Dodo.Primitives;

using Microsoft.EntityFrameworkCore;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Interfaces;
using MyProjectTemplate.Infra.Data;

namespace MyProjectTemplate.Infra.Repositories;

public class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Uuid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> 
        GetAllAsync(PaginationParams pagination,
                    Expression<Func<T, bool>>? filter = null,
                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                    CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet;

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);
        else
            query = query.OrderBy(e => true);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        DbSet.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Uuid id)
    {
        var entity = await GetByIdAsync(id);

        if (entity != null)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync(Uuid id)
    {
        return await DbSet.FindAsync(id) != null;
    }
}
