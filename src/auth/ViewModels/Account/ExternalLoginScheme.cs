using System;
using Microsoft.AspNetCore.Authentication;
using Maw.TagHelpers;


namespace MawAuth.ViewModels.Account
{
    public class ExternalLoginScheme
    {
        public AuthenticationScheme ExternalAuth { get; }


        public SvgIcon? Icon
        {
            get
            {
                var scheme = ExternalAuth.Name.ToUpperInvariant();

                switch(scheme)
                {
                    case "GITHUB":
                        return SvgIcon.Github;
                    case "GOOGLE":
                        return SvgIcon.GooglePlus;
                    case "MICROSOFT":
                        return SvgIcon.Windows;
                    case "TWITTER":
                        return SvgIcon.Twitter;
                }

                return null;
            }
        }


        public ExternalLoginScheme(AuthenticationScheme authScheme)
        {
            ExternalAuth = authScheme ?? throw new ArgumentNullException(nameof(authScheme));
        }
    }
}