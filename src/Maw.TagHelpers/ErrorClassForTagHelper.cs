using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace Maw.TagHelpers
{
	[HtmlTargetElement("div", Attributes = ForAttributeName)]
	public class ErrorClassForTagHelper 
		: TagHelper
	{
		const string ForAttributeName = "maw-for";
		
		
		[HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }
		
		
		[HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
		
			
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			ModelStateEntry modelState;
			
			if (ViewContext.ViewData.ModelState.TryGetValue(For.Name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
					var att = output.Attributes.FirstOrDefault(x => string.Equals(x.Name, "class", StringComparison.OrdinalIgnoreCase));
                    var klass = "has-error";

					if (att != null)
					{
						klass = $" {klass}";
					}
					
                    output.Attributes.SetAttribute("class", klass);
                }
            }
		}
	}
}