using IdentityModel.Client;
using System.Threading.Tasks;

namespace Client.Service
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}
