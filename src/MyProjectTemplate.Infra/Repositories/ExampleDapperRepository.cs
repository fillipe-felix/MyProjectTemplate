using System.Linq.Expressions;

using Dapper;

using Dodo.Primitives;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Entities;
using MyProjectTemplate.Domain.Interfaces;
using MyProjectTemplate.Infra.Contracts;

namespace MyProjectTemplate.Infra.Repositories;

public class ExampleDapperRepository : IExampleDapperRepository
{
    private readonly IDapperWrapper _context;

    public ExampleDapperRepository(IDapperWrapper context)
    {
        _context = context;
    }

    public async Task<Example?> GetByIdAsync(Uuid id)
    {
        const string sql = @"
                            SELECT 
                                Id,
                                Name,
                                Description,
                                Date,
                                Location,
                                Latitude,
                                Longitude,
                                Difficulty,
                                Active,
                                CreatedAt,
                            FROM Examples
                            WHERE Id = @Id";

        var dp = new DynamicParameters();
        dp.Add("@Id", id);

        var result = await _context.QueryAsync<Example>(sql, dp);
        return result.FirstOrDefault();
    }

    public async Task<(IEnumerable<Example> Items, int TotalCount)> GetAllAsync(
        PaginationParams pagination,
        Expression<Func<Example, bool>>? filter = null,
        Func<IQueryable<Example>, IOrderedQueryable<Example>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var dp = new DynamicParameters();
        var pageSize = pagination?.PageSize ?? 10;
        var pageNumber = pagination?.PageNumber ?? 1;
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        dp.Add("@Offset", (pageNumber - 1) * pageSize);
        dp.Add("@PageSize", pageSize);

        var items = await _context.QueryAsync<Example>(@"
                            SELECT 
                                Id,
                                Name,
                                Description,
                                Date,
                                Location,
                                Latitude,
                                Longitude,
                                Difficulty,
                                Active,
                                CreatedAt
                            FROM Examples
                            ORDER BY CreatedAt DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;", dp);

        var total = await _context.QueryAsync<int>(@"SELECT COUNT(1) FROM Examples;", null);

        return (items, total.FirstOrDefault());
    }

    public async Task<Example> AddAsync(Example entity)
    {
        const string sql = @"
                INSERT INTO Examples
                    (Id, Name, Description, Date, Location, Latitude, Longitude, Difficulty, Active, CreatedAt)
                VALUES
                    (@Id, @Name, @Description, @Date, @Location, @Latitude, @Longitude, @Difficulty, 1, GETUTCDATE());";

        var dp = new DynamicParameters();
        dp.Add("@Id", entity.Id);
        dp.Add("@Name", entity.Name);
        dp.Add("@Description", entity.Description);
        dp.Add("@Date", entity.Date);
        dp.Add("@Location", entity.Location);
        dp.Add("@Latitude", entity.Latitude);
        dp.Add("@Longitude", entity.Longitude);
        dp.Add("@Difficulty", entity.Difficulty);

        await _context.ExecuteAsync(sql, dp);
        return entity;
    }

    public async Task UpdateAsync(Example entity)
    {
        const string sql = @"
                    UPDATE Examples
                    SET
                        Name = @Name,
                        Description = @Description,
                        Date = @Date,
                        Location = @Location,
                        Latitude = @Latitude,
                        Cost = @Cost,
                        Difficulty = @Difficulty
                    WHERE Id = @Id;";

        var dp = new DynamicParameters();
        dp.Add("@Id", entity.Id);
        dp.Add("@Name", entity.Name);
        dp.Add("@Description", entity.Description);
        dp.Add("@Date", entity.Date);
        dp.Add("@Location", entity.Location);
        dp.Add("@Latitude", entity.Latitude);
        dp.Add("@Longitude", entity.Longitude);
        dp.Add("@Difficulty", entity.Difficulty);

        await _context.ExecuteAsync(sql, dp);
    }

    public async Task DeleteAsync(Uuid id)
    {
        const string sql = @"DELETE FROM Examples WHERE Id = @Id;";
        var dp = new DynamicParameters();
        dp.Add("@Id", id);

        await _context.ExecuteAsync(sql, dp);
    }

    public async Task<bool> ExistsAsync(Uuid id)
    {
        const string sql = @"SELECT 1 FROM Examples WHERE Id = @Id;";
        var dp = new DynamicParameters();
        dp.Add("@Id", id);

        var result = await _context.QueryAsync<int>(sql, dp);
        return result.Any();
    }
}
