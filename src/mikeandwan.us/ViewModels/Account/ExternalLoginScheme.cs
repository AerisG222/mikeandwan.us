using System;
using Maw.TagHelpers;
using Microsoft.AspNetCore.Http.Authentication;


namespace MawMvcApp.ViewModels.Account
{
    public class ExternalLoginScheme
    {
        public AuthenticationDescription ExternalAuth { get; }


        public SvgIcon? Icon
        {
            get
            {
                var scheme = ExternalAuth.AuthenticationScheme.ToLower();

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


        public ExternalLoginScheme(AuthenticationDescription authDesc)
        {
            if(authDesc == null)
            {
                throw new ArgumentNullException(nameof(authDesc));
            }

            ExternalAuth = authDesc;
        }
    }
}