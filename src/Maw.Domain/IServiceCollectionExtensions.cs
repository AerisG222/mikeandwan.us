using Microsoft.Extensions.DependencyInjection;
using Maw.Domain.Blogs;
using Maw.Domain.Identity;
using Maw.Domain.Photos;
using Maw.Domain.Videos;


namespace Maw.Domain
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMawServices(this IServiceCollection services)
        {
            services
                .AddScoped<IBlogService, BlogService>()
                .AddScoped<IPhotoService, PhotoService>()
                .AddScoped<IVideoService, VideoService>();

            return services;
        }
    }
}
