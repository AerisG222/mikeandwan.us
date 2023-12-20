using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maw.TagHelpers;

[HtmlTargetElement("a", Attributes = AttributeName)]
public class UrlAuthTagHelper
    : TagHelper
{
    const string AttributeName = "maw-auth-url";
    readonly Uri _authUri;

    [HtmlAttributeName(AttributeName)]
    public string? Url { get; set; }

    public UrlAuthTagHelper(TagHelperConfig config)
    {
        if (config.AuthUrl == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        _authUri = new Uri(config.AuthUrl);
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        Uri dest = new(_authUri, Url);

        output.Attributes.SetAttribute("href", dest);
    }
}
