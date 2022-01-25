using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Maw.Data.Identity;
using Maw.Data.Abstractions;
using Maw.Domain.Models.Identity;

namespace Maw.Data;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMawDataServices(
        this IServiceCollection services,
        string connString)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);

        services
            .AddScoped<IBlogRepository>(x => new BlogRepository(connString))
            .AddScoped<IUserRepository>(x => new UserRepository(connString))
            .AddScoped<IPhotoRepository>(x => new PhotoRepository(connString))
            .AddScoped<IVideoRepository>(x => new VideoRepository(connString))
            .AddScoped<IUserStore<MawUser>, MawUserStore>()
            .AddScoped<IRoleStore<MawRole>, MawRoleStore>();

        return services;
    }
}
