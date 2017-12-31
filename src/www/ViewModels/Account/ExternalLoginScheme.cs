using System;
using Microsoft.AspNetCore.Authentication;
using MawMvcApp.TagHelpers;


namespace MawMvcApp.ViewModels.Account
{
    public class ExternalLoginScheme
    {
        public AuthenticationScheme ExternalAuth { get; }


        public SvgIcon? Icon
        {
            get
            {
                var scheme = ExternalAuth.Name.ToLower();

                switch(scheme)
                {
                    case "github":
                        return SvgIcon.Github;
                    case "google":
                        return SvgIcon.GooglePlus;
                    case "microsoft":
                        return SvgIcon.Windows;
                    case "twitter":
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