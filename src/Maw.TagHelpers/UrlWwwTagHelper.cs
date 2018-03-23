using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace Maw.TagHelpers
{
	[HtmlTargetElement("a", Attributes = AttributeName)]
	public class UrlWwwTagHelper
		: TagHelper
	{
		const string AttributeName = "maw-www-url";
        static readonly Uri LocalBaseUri = new Uri("https://localhost:5001/");
        static readonly Uri ProductionBaseUri = new Uri("https://www.mikeandwan.us/");


        [ViewContext]
        public ViewContext ViewContext { get; set; }


		[HtmlAttributeName(AttributeName)]
		public string Url { get; set; }


		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
            Uri dest = null;
			var req = ViewContext.HttpContext.Request.Host;

            if(string.Equals(req.Host, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                dest = new Uri(LocalBaseUri, Url);
            }
            else
            {
                dest = new Uri(ProductionBaseUri, Url);
            }

			output.Attributes.SetAttribute("href", dest);
		}
	}
}