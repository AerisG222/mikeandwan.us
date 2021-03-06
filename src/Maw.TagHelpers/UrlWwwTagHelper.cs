using System;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace Maw.TagHelpers
{
    [HtmlTargetElement("a", Attributes = AttributeName)]
    public class UrlWwwTagHelper
        : TagHelper
    {
        const string AttributeName = "maw-www-url";
        readonly Uri _wwwUri;


        [HtmlAttributeName(AttributeName)]
        public string Url { get; set; }


        public UrlWwwTagHelper(TagHelperConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _wwwUri = new Uri(config.WwwUrl);
        }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            Uri dest = new Uri(_wwwUri, Url);

            output.Attributes.SetAttribute("href", dest);
        }
    }
}
