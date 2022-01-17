using System;
using Microsoft.Extensions.DependencyInjection;

namespace Maw.TagHelpers;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMawTagHelpers(this IServiceCollection services, Action<TagHelperConfig> configureOpts)
    {
        if (configureOpts == null)
        {
            throw new ArgumentNullException(nameof(configureOpts));
        }

        var config = new TagHelperConfig();

        configureOpts(config);

        services.AddSingleton<TagHelperConfig>(config);

        return services;
    }
}
