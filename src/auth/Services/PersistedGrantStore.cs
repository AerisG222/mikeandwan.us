using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;
using IdentityServer4.Extensions;
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
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _connString = config.ConnectionString;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            if(filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            filter.Validate();

            _log.LogDebug("getting all grants for subject: {SubjectId}, client: {ClientId}, session: {SessionId}, type: {Type}",
                filter.SubjectId,
                filter.ClientId,
                filter.SessionId,
                filter.Type);

            return RunAsync(conn =>
                conn.QueryAsync<PersistedGrant>(
                    "SELECT * FROM idsrv.get_persisted_grants(@subjectId, @sessionId, @clientId, @type);",
                    filter
                )
            );
        }


        public Task<PersistedGrant> GetAsync(string key)
        {
            if(string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _log.LogDebug("getting grant for key: {Key}", key);

            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<PersistedGrant>(
                    "SELECT * FROM idsrv.get_persisted_grant(@key);",
                    new
                    {
                        key
                    }
                )
            );
        }


        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            if(filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            filter.Validate();

            _log.LogDebug("removing all grants for subject: {SubjectId}, client: {ClientId}, session: {SessionId}, type: {Type}",
                filter.SubjectId,
                filter.ClientId,
                filter.SessionId,
                filter.Type);

            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<long>(
                    "SELECT * FROM idsrv.delete_persisted_grants(@subjectId, @sessionId, @clientId, @type);",
                    filter
                )
            );
        }


        public Task RemoveAsync(string key)
        {
            if(string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _log.LogDebug("removing grant for key: {Key}", key);

            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<long>(
                    "SELECT * FROM idsrv.delete_persisted_grant(@key);",
                    new
                    {
                        key
                    }
                )
            );
        }


        public Task StoreAsync(PersistedGrant grant)
        {
            if (grant == null)
            {
                throw new ArgumentNullException(nameof(grant));
            }

            _log.LogDebug("storing grant for key: {Key}, type: {Type}, subject: {SubjectId}, client: {ClientId}, session: {SessionId}",
                grant.Key,
                grant.Type,
                grant.SubjectId,
                grant.ClientId,
                grant.SessionId
            );

            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<long>(
                    "SELECT * FROM idsrv.save_persisted_grant(@Key, @Type, @SubjectId, @SessionId, @ClientId, @Description, @CreationTime, @Expiration, @ConsumedTime, @Data);",
                    grant
                )
            );
        }


        async Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> queryData)
        {
            using var conn = GetConnection();

            await conn.OpenAsync().ConfigureAwait(false);

            return await queryData(conn).ConfigureAwait(false);
        }


        async Task RunAsync(Func<IDbConnection, Task> executeStatement)
        {
            using var conn = GetConnection();

            await conn.OpenAsync().ConfigureAwait(false);

            await executeStatement(conn).ConfigureAwait(false);
        }


        DbConnection GetConnection()
        {
            return new NpgsqlConnection(_connString);
        }
    }
}
