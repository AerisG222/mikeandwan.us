using System.Data;
using System.Data.Common;
using Npgsql;
using MawAuth.Models;

namespace MawAuth.Services;

public abstract class BaseStore
{
    readonly string _connString;

    protected ILogger Log { get; }

    protected BaseStore(
        StoreConfig config,
        ILogger log)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        _connString = config.ConnectionString;
        Log = log ?? throw new ArgumentNullException(nameof(log));
    }

    protected async Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> queryData)
    {
        if(queryData == null)
        {
            throw new ArgumentNullException(nameof(queryData));
        }

        using var conn = GetConnection();

        await conn.OpenAsync();

        return await queryData(conn);
    }

    protected async Task RunAsync(Func<IDbConnection, Task> executeStatement)
    {
        if(executeStatement == null)
        {
            throw new ArgumentNullException(nameof(executeStatement));
        }

        using var conn = GetConnection();

        await conn.OpenAsync();

        await executeStatement(conn);
    }

    DbConnection GetConnection()
    {
        return new NpgsqlConnection(_connString);
    }
}
