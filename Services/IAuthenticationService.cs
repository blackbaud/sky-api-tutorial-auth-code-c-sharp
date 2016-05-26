using System;
using System.Net.Http;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IAuthenticationService
    {   
        HttpResponseMessage ExchangeCodeForAccessToken(string code);
        Uri GetAuthorizationUri();
        bool IsAuthenticated();
        void LogOut();
        HttpResponseMessage RefreshAccessToken();
    }
}