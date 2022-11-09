using Blackbaud.AuthCodeFlowTutorial.Models;
using System.Net.Http;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface ISessionService
    {   
        void SetTokens(RefreshTokenResponseModel response);
        void ClearTokens();
        string GetAccessToken();
        string GetRefreshToken();
        DateTimeOffset? GetRefrehExpires();
        DateTimeOffset? GetAccessExpires();
        string GetStateVerifier(string state);
        void SetStateVerifier(string state, string verifier);
        void ClearStateVerifier(string state);
    }
}