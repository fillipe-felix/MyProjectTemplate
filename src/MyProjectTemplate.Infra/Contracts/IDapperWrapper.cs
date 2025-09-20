using System.Data;

using Dapper;

namespace MyProjectTemplate.Infra.Contracts;

public interface IDapperWrapper
{
    Task<IEnumerable<T>> QueryAsync<T>(
        string sql, DynamicParameters? dynamicParameters);
    
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        string sql, Func<TFirst, TSecond, TReturn> map, 
        object? param = null, IDbTransaction? transaction = null, 
        bool buffered = true, string splitOn = "Id", 
        int? commandTimeout = null, CommandType? commandType = null);
    
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
        string sql, Func<TFirst, TSecond, TThird, TReturn> map, 
        object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, string splitOn = "Id", 
        int? commandTimeout = null, CommandType? commandType = null);
    
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, 
        object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, string splitOn = "Id", 
        int? commandTimeout = null, CommandType? commandType = null);
    
    Task<IDbTransaction> BeginTransactionAsync();
    Task ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);
    Task<int> ExecureAndReturnKeyAsync(string sql, object? param = null, IDbTransaction? transaction = null);
    Task CommitAsync();
    Task RollbackAsync();
}
