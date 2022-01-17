using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maw.TagHelpers;

[HtmlTargetElement("a", Attributes = AttributeName)]
public class UrlAuthTagHelper
    : TagHelper
{
    const string AttributeName = "maw-auth-url";
    readonly Uri _authUri;

    [HtmlAttributeName(AttributeName)]
    public string Url { get; set; }

    public UrlAuthTagHelper(TagHelperConfig config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        _authUri = new Uri(config.AuthUrl);
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        Uri dest = new Uri(_authUri, Url);

        output.Attributes.SetAttribute("href", dest);
    }
}
