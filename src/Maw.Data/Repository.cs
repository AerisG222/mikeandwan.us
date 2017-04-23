using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;


namespace Maw.Data
{
    public abstract class Repository
    {
        string _connString;


        public Repository(string connectionString)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connString = connectionString;
        }


        // http://www.joesauve.com/async-dapper-and-async-sql-connection-management/
        protected async Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> queryData)
        {
            using(var conn = new NpgsqlConnection(_connString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                return await queryData(conn).ConfigureAwait(false);
            }
        }


        protected async Task RunAsync(Func<IDbConnection, Task> executeStatement)
        {
            using(var conn = new NpgsqlConnection(_connString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                await executeStatement(conn).ConfigureAwait(false);
            }
        }
    }
}
