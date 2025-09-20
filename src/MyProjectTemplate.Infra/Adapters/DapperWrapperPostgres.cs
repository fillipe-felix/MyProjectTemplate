using System.Data;
using System.Diagnostics.CodeAnalysis;

using Dapper;

using MyProjectTemplate.Infra.Contracts;

using Npgsql;

namespace MyProjectTemplate.Infra.Adapters;

[ExcludeFromCodeCoverage]
public class DapperWrapperPostgres: IDapperWrapper
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection;
    private IDbTransaction _transaction = null;

    public DapperWrapperPostgres(DbRepositoryAdapterConfiguration configuration)
    {
        _connectionString = configuration.SqlConnectionString 
                            ?? throw new ArgumentNullException(nameof(configuration.SqlConnectionString));
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters? dynamicParameters)
    {
        IEnumerable<T> result;

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            result = await connection.QueryAsync<T>(sql, dynamicParameters);
        }
        
        return result;
    }
    
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters? dynamicParameters, IDbTransaction transaction)
    {
        IEnumerable<T>? result;
        result = await _connection.QueryAsync<T>(sql, dynamicParameters, transaction);
        
        return result;
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        IEnumerable<TReturn>? result;

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            result = await connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, transaction, 
                buffered, splitOn, commandTimeout, commandType);
        }
        
        return result;
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
        string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        IEnumerable<TReturn>? result;

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            result = await connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(sql, map, param, transaction, 
                buffered, splitOn, commandTimeout, commandType);
        }
        
        return result;
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, 
        bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        IEnumerable<TReturn>? result;

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            result = await connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, transaction, 
                buffered, splitOn, commandTimeout, commandType);
        }
        
        return result;
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        if (_transaction is null)
        {
            await OpenAsync();
            _transaction = _connection.BeginTransaction();
        }

        return _transaction;
    }

    public async Task ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        if (transaction is null)
        {
            await OpenAsync();
            await _connection.ExecuteAsync(sql, param, transaction);
            
            await _connection.CloseAsync();
            
            return;
        }
        
        await transaction.Connection.ExecuteAsync(sql, param, transaction);
    }

    public async Task<int> ExecureAndReturnKeyAsync(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        if (transaction is null)
        {
            await OpenAsync();
            var key = await _connection.ExecuteAsync(sql, param, transaction);

            await _connection.CloseAsync();

            return key;
        }
        
        return await transaction.Connection.ExecuteAsync(sql, param, transaction);
    }

    public async Task CommitAsync()
    {
        _transaction?.Commit();
        await _connection.CloseAsync();
        _transaction = null;
        _connection = null;
    }

    public async Task RollbackAsync()
    {
        _transaction?.Rollback();

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
        
        _transaction = null;
        _connection = null;
    }

    private async Task<NpgsqlConnection> OpenAsync()
    {
        if (_connection is not null)
        {
            return _connection;
        }

        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        _connection = connection;
        
        return _connection;
    }
}