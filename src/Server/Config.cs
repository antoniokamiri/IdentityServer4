using IdentityServer4.Models;
using System.Collections.Generic;

namespace Server
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                new []
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResource
                    {
                        Name = "Roles",
                        UserClaims = new List<string> {"role"}
                    }
                };

        public static IEnumerable<ApiScope> ApiScopes =>
                new []
                {
                    new ApiScope("myApi.read"),
                    new ApiScope("myApi.write"),
                    new ApiScope("myApi.update"),
                    new ApiScope("myApi.delete"),
                };

        public static IEnumerable<ApiResource> ApiResources =>
                new []
                {
                    new ApiResource("myApi")
                    {
                        Scopes = new List<string>{ "myApi.read","myApi.write","myApi.update","myApi.delete" },
                        ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) },
                        UserClaims = new List<string> {"role"}
                    }
                };

        public static IEnumerable<Client> Clients =>
                new []
                {
                    //M2M client
                    new Client
                    {
                        ClientId = "cwm.client",
                        ClientName = "Client Credentials Client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedScopes = { "myApi.read", "myApi.write", "myApi.update" }
                    },
                    //Interative client using code flow

                    new Client
                    {
                        ClientId = "Interative.client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = { new Secret("secret1".Sha256()) },
                        RedirectUris = {"https://localhost:5001/signin-oidc"},
                        FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",
                        PostLogoutRedirectUris = {"https://localhost:5001/signout-callback-oidc"},
                        AllowOfflineAccess = true,
                        AllowedScopes = {"openid", "profile", "myApi.read"},
                        RequirePkce = true,
                        RequireConsent = true,
                        AllowPlainTextPkce = false
                    }
                };
    }
}
