using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Npgsql;
using Maw.Domain;

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
            using(var conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                return await queryData(conn).ConfigureAwait(false);
            }
        }


        protected async Task RunAsync(Func<IDbConnection, Task> executeStatement)
        {
            using(var conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                await executeStatement(conn).ConfigureAwait(false);
            }
        }


        protected T GetValueOrDefault<T>(object value)
        {
            return value == null ? default(T) : (T)value;
        }


        protected MultimediaInfo BuildMultimediaInfo(dynamic path, dynamic width, dynamic height, dynamic size)
        {
            if(path == null)
            {
                return null;
            }

            var mi = new MultimediaInfo();

            mi.Path = GetValueOrDefault<string>(path);
            mi.Width = GetValueOrDefault<short>(width);
            mi.Height = GetValueOrDefault<short>(height);
            mi.Size = size == null ? 0 : Convert.ToInt64(size);

            return mi;
        }


        DbConnection GetConnection()
        {
            DbConnection dbConn = new NpgsqlConnection(_connString);

            return new ProfiledDbConnection(dbConn, MiniProfiler.Current);
        }
    }
}
