using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using MawAuth.Models;

namespace MawAuth.Services
{
    public class PersistedGrantStore
        : BaseStore, IPersistedGrantStore
    {
        public PersistedGrantStore(StoreConfig config, ILogger<PersistedGrantStore> log)
            : base(config, log)
        {

        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            if(filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            filter.Validate();

            Log.LogDebug("getting all grants for subject: {SubjectId}, client: {ClientId}, session: {SessionId}, type: {Type}",
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

            Log.LogDebug("getting grant for key: {Key}", key);

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

            Log.LogDebug("removing all grants for subject: {SubjectId}, client: {ClientId}, session: {SessionId}, type: {Type}",
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

            Log.LogDebug("removing grant for key: {Key}", key);

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

            Log.LogDebug("storing grant for key: {Key}, type: {Type}, subject: {SubjectId}, client: {ClientId}, session: {SessionId}",
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
    }
}
