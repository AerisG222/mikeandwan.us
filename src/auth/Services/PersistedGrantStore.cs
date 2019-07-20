using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Npgsql;
using MawAuth.Models;


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

            return InternalGetGrants(null, subjectId);
        }


        public async Task<PersistedGrant> GetAsync(string key)
        {
            _log.LogDebug($"getting all grants for key: {key}");

            var grants = await InternalGetGrants(key).ConfigureAwait(false);

            return grants.SingleOrDefault();
        }


        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            _log.LogDebug($"removing all grants for subject: {subjectId} and client: {clientId}");

            return InternalRemoveGrants(subjectId: subjectId, clientId: clientId);
        }


        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _log.LogDebug($"removing all grants for subject: {subjectId} and client: {clientId} and type: {type}");

            return InternalRemoveGrants(subjectId: subjectId, clientId: clientId, type: type);
        }


        public Task RemoveAsync(string key)
        {
            _log.LogDebug($"removing grant for key: {key} (if expired)");

            return InternalRemoveGrants(key: key);
        }


        public Task StoreAsync(PersistedGrant grant)
        {
            if(grant == null)
            {
                throw new ArgumentNullException(nameof(grant));
            }

            _log.LogDebug($"storing grant for key: {grant.Key}, type: {grant.Type}, subject: {grant.SubjectId}, client: {grant.ClientId}");

            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<long>(
					"SELECT * FROM idsrv.save_persisted_grant(@Key, @Type, @SubjectId, @ClientId, @CreationTime, @Expiration, @Data);",
					grant
				)
			);
        }


        Task<long> InternalRemoveGrants(string key = null, string subjectId = null, string clientId = null, string type = null)
        {
            return RunAsync(conn =>
				conn.QuerySingleOrDefaultAsync<long>(
					"SELECT * FROM idsrv.delete_persisted_grants(@key, @subjectId, @clientId, @type);",
					new {
                        key,
                        subjectId,
                        clientId,
                        type
                    }
				)
			);
        }


        Task<IEnumerable<PersistedGrant>> InternalGetGrants(string key = null, string subjectId = null)
        {
            return RunAsync(conn =>
				conn.QueryAsync<PersistedGrant>(
					"SELECT * FROM idsrv.get_persisted_grants(@key, @subjectId);",
					new {
                        key,
                        subjectId
                    }
				)
			);
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
