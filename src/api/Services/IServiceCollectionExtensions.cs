using Microsoft.Extensions.DependencyInjection;
using MawApi.Services.Photos;


namespace MawApi.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMawApiServices(this IServiceCollection services)
        {
            return services
                // shared
                .AddSingleton<UrlBuilderService>()

                // photos
                .AddSingleton<LegacyMultimediaInfoAdapter>()
                .AddSingleton<LegacyPhotoAdapter>()
                .AddSingleton<LegacyPhotoCategoryAdapter>()
                .AddSingleton<MultimediaInfoAdapter>()
                .AddSingleton<PhotoAdapter>()
                .AddSingleton<PhotoCategoryAdapter>()
                .AddSingleton<PhotoUrlBuilderService>();
        }
    }
}
