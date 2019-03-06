using Microsoft.Extensions.DependencyInjection;
using MawApi.Services.Photos;
using MawApi.Services.Videos;

namespace MawApi.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMawApiServices(this IServiceCollection services)
        {
            return services
                // shared
                .AddSingleton<UrlBuilderService>()
                .AddSingleton<MawApi.Services.Photos.LegacyMultimediaInfoAdapter>()
                .AddSingleton<MawApi.Services.Videos.LegacyMultimediaInfoAdapter>()
                .AddSingleton<MultimediaInfoAdapter>()

                // photos
                .AddSingleton<LegacyPhotoAdapter>()
                .AddSingleton<LegacyPhotoCategoryAdapter>()
                .AddSingleton<PhotoAdapter>()
                .AddSingleton<PhotoCategoryAdapter>()
                .AddSingleton<PhotoUrlBuilderService>()
                .AddSingleton<PhotoMultimediaInfoAdapter>()

                // videos
                .AddSingleton<LegacyVideoAdapter>()
                .AddSingleton<LegacyVideoCategoryAdapter>()
                .AddSingleton<VideoAdapter>()
                .AddSingleton<VideoCategoryAdapter>()
                .AddSingleton<VideoUrlBuilderService>();
        }
    }
}
