using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Service
{
    public class TokenService : ITokenService
    {
        public readonly IOptions<IdentityServerSettings> _IdentityServerSettings;
        public readonly DiscoveryDocumentResponse _DiscoveryDocumentResponse;
        public readonly HttpClient _HttpClient;

        public TokenService(IOptions<IdentityServerSettings> identityServerSettings)
        {
            _IdentityServerSettings = identityServerSettings;
            _HttpClient = new HttpClient();
            _DiscoveryDocumentResponse = _HttpClient.GetDiscoveryDocumentAsync(_IdentityServerSettings.Value.DiscoveryUrl).Result;

            if(_DiscoveryDocumentResponse.IsError)
            {
                throw new Exception("Unable to get Discovery Exception", _DiscoveryDocumentResponse.Exception);
            }
            
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            var tokenResponse = await _HttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _DiscoveryDocumentResponse.TokenEndpoint,
                Scope = scope,
                ClientId = _IdentityServerSettings.Value.ClientName,
                ClientSecret = _IdentityServerSettings.Value.ClientPassword,
            });

            if(tokenResponse.IsError)
            {
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse;
        }
    }
}
