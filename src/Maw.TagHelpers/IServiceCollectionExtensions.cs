using Microsoft.Extensions.DependencyInjection;

namespace Maw.TagHelpers;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMawTagHelpers(this IServiceCollection services, Action<TagHelperConfig> configureOpts)
    {
        ArgumentNullException.ThrowIfNull(configureOpts);

        var config = new TagHelperConfig();

        configureOpts(config);

        services.AddSingleton<TagHelperConfig>(config);

        return services;
    }
}
