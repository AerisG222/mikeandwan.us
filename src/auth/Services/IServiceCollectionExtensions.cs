using Duende.IdentityServer.Stores;
using MawAuth.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MawAuth.Services;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMawIdentityServerServices(this IServiceCollection services, string connString, string signingCertDir)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);

        services
            .AddSingleton<StoreConfig>(new StoreConfig(connString))
            .AddScoped<IPersistedGrantStore, PersistedGrantStore>()
            .AddScoped<ISigningKeyStore, SigningKeyStore>();

        return services;
    }
}
