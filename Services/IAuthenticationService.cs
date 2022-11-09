using Blackbaud.AuthCodeFlowTutorial.Models;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IAuthenticationService
    {
        Task<RefreshTokenResponseModel> ExchangeCodeForAccessToken(string code, string state, CancellationToken cancellationToken);
        Uri GetAuthorizationUri();
        bool IsAuthenticated();
        bool IsAccessTokenValid();
        bool HasValidRefreshToken();
        void LogOut();
        Task<RefreshTokenResponseModel> RefreshAccessToken(CancellationToken cancellationToken);
    }
}