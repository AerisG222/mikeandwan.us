using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Npgsql;


namespace MawAuth.Services
{
    public class PersistedGrantStore
        : IPersistedGrantStore
    {
        string _connString;


        public PersistedGrantStore(string connectionString)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connString = connectionString;
        }


        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return RunAsync(conn => {
				return conn.QueryAsync<PersistedGrant>(
					@"SELECT *
					    FROM idsrv.persisted_grant
					   WHERE subject_id = @subjectId
					   ORDER BY creation_time;",
					new { subjectId = subjectId }
				);
			});
        }


        public Task<PersistedGrant> GetAsync(string key)
        {
            return RunAsync(conn => {
				return conn.QuerySingleOrDefaultAsync<PersistedGrant>(
					@"SELECT *
					    FROM idsrv.persisted_grant
					   WHERE key = @key;",
					new { key = key }
				);
			});
        }


        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"DELETE FROM idsrv.persisted_grant
					   WHERE subject_id = @subjectId
                         AND client_id = @clientId;",
					new {
                        subjectId = subjectId,
                        clientId = clientId
                     }
				);
			});
        }


        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"DELETE FROM idsrv.persisted_grant
					   WHERE subject_id = @subjectId
                         AND client_id = @clientId
                         AND type = @type;",
					new {
                        subjectId = subjectId,
                        clientId = clientId,
                        type = type
                     }
				);
			});
        }


        public Task RemoveAsync(string key)
        {
            return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"DELETE FROM idsrv.persisted_grant
					   WHERE key = @key;",
					new { key = key }
				);
			});
        }


        public Task StoreAsync(PersistedGrant grant)
        {
            return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"INSERT INTO idsrv.persisted_grant
                           (
                               key,
                               type,
                               subject_id,
                               client_id,
                               creation_time,
                               expiration,
                               data
                           )
                      VALUES
                           (
                               @Key,
                               @Type,
                               @SubjectId,
                               @ClientId,
                               @CreationTime,
                               @Expiration,
                               @Data
                           )
                      ON CONFLICT (key)
                      DO UPDATE
                            SET type = @Type,
                                subject_id = @SubjectId,
                                client_id = @ClientId,
                                creation_time = @CreationTime,
                                expiration = @Expiration,
                                data = @Data;",
					grant
				);
			});
        }


        async Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> queryData)
        {
            using(var conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                return await queryData(conn).ConfigureAwait(false);
            }
        }


        async Task RunAsync(Func<IDbConnection, Task> executeStatement)
        {
            using(var conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                await executeStatement(conn).ConfigureAwait(false);
            }
        }


        DbConnection GetConnection()
        {
            return new NpgsqlConnection(_connString);
        }
    }
}
