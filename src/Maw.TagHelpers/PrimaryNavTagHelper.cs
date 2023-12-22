using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maw.TagHelpers;

[HtmlTargetElement("li", Attributes = ControllerAttributeName)]
[HtmlTargetElement("li", Attributes = ActionAttributeName)]
[HtmlTargetElement("li", Attributes = LinkTextAttributeName)]
[HtmlTargetElement("li", Attributes = IsActiveAttributeName)]
[HtmlTargetElement("li", Attributes = RouteValuesDictionaryName)]
[HtmlTargetElement("li", Attributes = RouteValuesPrefix + "*")]
public class PrimaryNavTagHelper
    : TagHelper
{
    const string PageAttributeName = "maw-page";
    const string ActionAttributeName = "maw-action";
    const string ControllerAttributeName = "maw-controller";
    const string LinkTextAttributeName = "maw-text";
    const string IsActiveAttributeName = "maw-active";
    const string RouteValuesDictionaryName = "maw-all-route-data";
    const string RouteValuesPrefix = "maw-route-";
    readonly IHtmlGenerator _htmlGenerator;
    IDictionary<string, string> _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    [HtmlAttributeName(PageAttributeName)]
    public string? Page { get; set; }

    [HtmlAttributeName(ControllerAttributeName)]
    public string? Controller { get; set; }

    [HtmlAttributeName(ActionAttributeName)]
    public string? Action { get; set; }

    [HtmlAttributeName(LinkTextAttributeName)]
    public string? LinkText { get; set; }

    [HtmlAttributeName(IsActiveAttributeName)]
    public bool IsActive { get; set; } = false;


    [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
    public IDictionary<string, string> RouteValues
    {
        get { return _routeValues; }
        set { _routeValues = value; }
    }

    public PrimaryNavTagHelper(IHtmlGenerator htmlGenerator)
    {
        ArgumentNullException.ThrowIfNull(htmlGenerator);

        _htmlGenerator = htmlGenerator;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (Action == null && Page == null)
        {
            throw new InvalidOperationException("Action or Page must be specified");
        }

        LinkText ??= string.Empty;

        if (IsActive)
        {
            output.Attributes.Merge("class", "active");
        }

        var routeValues = RouteValues.ToDictionary(
                kvp => kvp.Key,
                kvp => (object)kvp.Value,
                StringComparer.OrdinalIgnoreCase);

        TagBuilder? anchor = null;

        if(Page != null)
        {
            anchor = _htmlGenerator.GeneratePageLink(ViewContext, LinkText, Page, Controller, null, null, null, routeValues, new { @class = "nav-link px-3" });
        }
        else
        {
            var action = Action!.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            anchor = _htmlGenerator.GenerateActionLink(ViewContext, LinkText, action, Controller, null, null, null, routeValues, new { @class = "nav-link px-3" });
        }

        anchor.InnerHtml.AppendHtml(await output.GetChildContentAsync());
        output.Content.AppendHtml(anchor);
    }
}
