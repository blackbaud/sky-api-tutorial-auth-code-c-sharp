using System;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IAuthenticationService
    {   
        void GetAccessToken(string code);
        Uri GetAuthorizationUri();
        bool IsAuthenticated();
        void LogOut();
        dynamic RefreshTokens();
        
    }
}