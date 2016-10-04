using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace Maw.TagHelpers
{
	[HtmlTargetElement("li", Attributes = ControllerAttributeName)]
	[HtmlTargetElement("li", Attributes = ActionAttributeName)]
	[HtmlTargetElement("li", Attributes = LinkTextAttributeName)]
	[HtmlTargetElement("li", Attributes = IconAttributeName)]
	[HtmlTargetElement("li", Attributes = IsActiveAttributeName)]
	[HtmlTargetElement("li", Attributes = RouteValuesDictionaryName)]
	[HtmlTargetElement("li", Attributes = RouteValuesPrefix + "*")]
	public class PrimaryNavTagHelper
		: TagHelper
	{
		const string ActionAttributeName = "maw-action";
		const string ControllerAttributeName = "maw-controller";
		const string LinkTextAttributeName = "maw-text";
		const string IconAttributeName = "maw-icon";
		const string IsActiveAttributeName = "maw-active";
		const string RouteValuesDictionaryName = "maw-all-route-data";
		const string RouteValuesPrefix = "maw-route-";
		readonly IHtmlGenerator _htmlGenerator;
		
		
		[HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
		
		
		[HtmlAttributeName(ControllerAttributeName)]	
		public string Controller { get; set; }
		
		
		[HtmlAttributeName(ActionAttributeName)]	
		public string Action { get; set; }
		
		
		[HtmlAttributeName(LinkTextAttributeName)]	
		public string LinkText { get; set; }
		
		
		[HtmlAttributeName(IconAttributeName)]	
		public string Icon { get; set; }
		
		
		[HtmlAttributeName(IsActiveAttributeName)]	
		public bool IsActive { get; set; }
		
		
		[HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			
		
		public PrimaryNavTagHelper(IHtmlGenerator htmlGenerator)
		{
			if(htmlGenerator == null)
			{
				throw new ArgumentNullException(nameof(htmlGenerator));
			}
			
			_htmlGenerator = htmlGenerator;
		}
		
		
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if(LinkText == null)
			{
				if(Icon == null) {
					throw new InvalidOperationException("This tag requires either the link text or icon to be set");
				}
				
				LinkText = string.Empty;
			}
			
			if(IsActive)
			{
                output.Attributes.Add("class", "active");
			}
			
			var routeValues = RouteValues.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)kvp.Value,
                    StringComparer.OrdinalIgnoreCase);
					
			var action = Action.Replace("-", string.Empty);
			var anchor = _htmlGenerator.GenerateActionLink(ViewContext, LinkText, action, Controller, null, null, null, routeValues, null);
			
			if(!string.IsNullOrEmpty(Icon))
			{
				var iconBuilder = new TagBuilder("i");
				
				iconBuilder.AddCssClass("fa");
            	iconBuilder.AddCssClass(Icon);
			    
				anchor.InnerHtml.AppendHtml(iconBuilder);
			}
			
			output.Content.AppendHtml(anchor);
		}
	}
}