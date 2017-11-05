using System;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace MawMvcApp.TagHelpers
{
    public static class TagHelperAttributeListExtensions
    {
        public static void Merge(this TagHelperAttributeList attributes, string name, string value)
        {
            var curr = attributes.FirstOrDefault(att => string.Equals(att.Name, name, StringComparison.Ordinal));

            if(curr == null)
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
}
