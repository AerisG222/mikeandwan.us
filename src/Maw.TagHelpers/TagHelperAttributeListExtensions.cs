using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maw.TagHelpers;

public static class TagHelperAttributeListExtensions
{
    public static void Merge(this TagHelperAttributeList attributes, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        var curr = attributes.FirstOrDefault(att => string.Equals(att.Name, name, StringComparison.Ordinal));

        if (curr == null)
        {
            attributes.Add(name, value);
        }
        else
        {
            attributes.Remove(curr);
            attributes.Add(name, $"{curr.Value} {value}");
        }
    }
}
