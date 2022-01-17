using System;
using Microsoft.AspNetCore.Authentication;
using Maw.TagHelpers;

namespace MawAuth.ViewModels.Account;

public class ExternalLoginScheme
{
    public AuthenticationScheme ExternalAuth { get; }

    public SvgIcon? Icon
    {
        get
        {
            var scheme = ExternalAuth.Name.ToUpperInvariant();

            return scheme switch
            {
                "GITHUB" => SvgIcon.Github,
                "GOOGLE" => SvgIcon.GooglePlus,
                "MICROSOFT" => SvgIcon.Windows,
                "TWITTER" => SvgIcon.Twitter,
                _ => null
            };
        }
    }

    public ExternalLoginScheme(AuthenticationScheme authScheme)
    {
        ExternalAuth = authScheme ?? throw new ArgumentNullException(nameof(authScheme));
    }
}
