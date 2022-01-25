using Dapper;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using MawAuth.Models;

namespace MawAuth.Services;

public class SigningKeyStore
    : BaseStore, ISigningKeyStore
{
    public SigningKeyStore(
        StoreConfig config,
        ILogger<SigningKeyStore> log)
        : base(config, log)
    {

    }

    public Task<IEnumerable<SerializedKey>> LoadKeysAsync()
    {
        Log.LogDebug("Getting all signing keys");

        return RunAsync(conn =>
            conn.QueryAsync<SerializedKey>(
                "SELECT * FROM idsrv.get_signing_keys();"
            )
        );
    }

    public Task StoreKeyAsync(SerializedKey key)
    {
        if(key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        Log.LogDebug("Storing signing key with id: {SigningKeyId}", key.Id);

        return RunAsync(conn =>
            conn.QuerySingleOrDefaultAsync<long>(
                "SELECT * FROM idsrv.save_signing_key(@id, @Version, @Created, @Algorithm, @IsX509Certificate, @DataProtected, @Data);",
                key
            )
        );
    }

    public Task DeleteKeyAsync(string id)
    {
        if(id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        Log.LogDebug("Deleting signing key with id: {SigningKeyId}", id);

        return RunAsync(conn =>
            conn.QuerySingleOrDefaultAsync<long>(
                "SELECT * FROM idsrv.delete_signing_key(@id);",
                new { id }
            )
        );
    }
}
