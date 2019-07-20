using System;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace Maw.TagHelpers
{
	[HtmlTargetElement("a", Attributes = IsActiveAttributeName)]
	public class SideNavTagHelper
		: TagHelper
	{
		const string IsActiveAttributeName = "maw-active";


		[HtmlAttributeName(IsActiveAttributeName)]
		public bool IsActive { get; set; }


		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
            if(output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

			var klass = "list-group-item";

			if(IsActive)
			{
				klass += " active";
			}

			var att = output.Attributes["class"];

			if(att == null)
			{
				output.Attributes.Add("class", klass);
			}
			else
			{
				var val = att.Value as string;

				if(string.IsNullOrWhiteSpace(val))
				{
					val = klass;
				}
				else
				{
					val = $"{val} {klass}";
				}

                output.Attributes.SetAttribute("class", val);
			}
		}
	}
}