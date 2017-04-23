using Microsoft.Extensions.DependencyInjection;
using Maw.Data.Identity;
using Maw.Domain.Blogs;
using Maw.Domain.Identity;
using Maw.Domain.Photos;
using Maw.Domain.Videos;


namespace Maw.Data
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMawDataRepositories(this IServiceCollection services, string connString)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);

            services
                .AddScoped<IBlogRepository>(x => new BlogRepository(connString))
                .AddScoped<IUserRepository>(x => new UserRepository(connString))
                .AddScoped<IPhotoRepository>(x => new PhotoRepository(connString))
                .AddScoped<IVideoRepository>(x => new VideoRepository(connString));

            return services;
        }
    }
}
