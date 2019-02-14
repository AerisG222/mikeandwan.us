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
                .AddSingleton<ImageAdapter>()
                .AddSingleton<PhotoAdapter>()
                .AddSingleton<PhotoCategoryAdapter>()
                .AddSingleton<PhotoUrlBuilderService>();
        }
    }
}
