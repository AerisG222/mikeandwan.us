using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maw.TagHelpers;

[HtmlTargetElement("p", Attributes = AlertTypeAttributeName)]
public class AlertTagHelper
    : TagHelper
{
    const string AlertTypeAttributeName = "maw-alert-type";

    [HtmlAttributeName(AlertTypeAttributeName)]
    public AlertType AlertType { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        var klass = "alert ";

        switch (AlertType)
        {
            case AlertType.Success:
                klass += "alert-success";
                break;
            case AlertType.Danger:
                klass += "alert-danger";
                break;
            case AlertType.Info:
                klass += "alert-info";
                break;
            case AlertType.Warning:
                klass += "alert-warning";
                break;
        }

        output.Attributes.Add("class", klass);
    }
}
