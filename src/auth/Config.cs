using IdentityServer4.Models;
using System.Collections.Generic;


namespace MawAuth
{
    public class Config
    {
        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("admin", "Administration APIs"),
                new ApiResource("blog", "Blog APIs"),
                new ApiResource("photo", "Photo APIs"),
                new ApiResource("video", "Video APIs")
            };
        }


        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "www.mikeandwan.us",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "admin", "blog", "photo", "profile", "video" }
                }
            };
        }
    }
}
