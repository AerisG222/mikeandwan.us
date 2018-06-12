using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MawAuth.Models;
using Microsoft.Extensions.Logging;
using Npgsql;


namespace MawAuth.Services
{
    public class PersistedGrantStore
        : IPersistedGrantStore
    {
        readonly string _connString;
        readonly ILogger _log;


        public PersistedGrantStore(StoreConfig config, ILogger<IdentityServerProfileService> log)
        {
            if(config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _connString = config.ConnectionString;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            _log.LogDebug($"getting all grants for subject: {subjectId}");

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
            _log.LogDebug($"getting all grants for key: {key}");

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
            _log.LogDebug($"removing all grants for subject: {subjectId} and client: {clientId}");

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
            _log.LogDebug($"removing all grants for subject: {subjectId} and client: {clientId} and type: {type}");

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
            _log.LogDebug($"removing grant for key: {key}");

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
            _log.LogDebug($"storing grant for key: {grant.Key}, type: {grant.Type}, subject: {grant.SubjectId}, client: {grant.ClientId}");

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
