using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    
    /// <summary>
    /// Contains business logic and helper methods that interact with the authentication provider.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {   
        
        private readonly IOptions<AppSettings> _appSettings;
        private ISessionService _sessionService;

        public AuthenticationService(IOptions<AppSettings> appSettings, ISessionService sessionService)
        {
            _appSettings = appSettings;
            _sessionService = sessionService;
        }
        
        
        /// <summary>
        /// Fetches access/refresh tokens from the provider.
        /// <param name="requestBody">Key-value attributes to be sent with the request.</param>
        /// <returns>The response from the provider.</returns>
        /// </summary>
        private HttpResponseMessage FetchTokens(Dictionary<string, string> requestBody) 
        {
            using (HttpClient client = new HttpClient()) 
            {   
                // Build token endpoint URL.
                string url = new Uri(new Uri(_appSettings.Value.AuthBaseUri), "token").ToString();
                
                // Set request headers.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization", "Basic " + Base64Encode(_appSettings.Value.AuthClientId + ":" + _appSettings.Value.AuthClientSecret));
                
                // Fetch tokens from auth server.
                HttpResponseMessage response = client.PostAsync(url, new FormUrlEncodedContent(requestBody)).Result;
                
                // Save the access/refresh tokens in the Session.
                _sessionService.SetTokens(response);
                
                return response;
            }
        }
        
        
        /// <summary>
        /// Fetches a new set of access/refresh tokens (from an authorization code).
        /// <param name="code">The authorization code contained within the provider's authorization response.</param>
        /// </summary>
        public HttpResponseMessage ExchangeCodeForAccessToken(string code)
        {
            return FetchTokens(new Dictionary<string, string>(){
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", _appSettings.Value.AuthRedirectUri }
            });
        }
        
        
        /// <summary>
        /// Refreshes the expired access token (from the refresh token stored in the session).
        /// </summary>
        public HttpResponseMessage RefreshAccessToken()
        {
            return FetchTokens(new Dictionary<string, string>(){
                { "grant_type", "refresh_token" },
                { "refresh_token", _sessionService.GetRefreshToken() }
            });
        }
        
        
        /// <summary>
        /// Builds and returns a string representative of the provider's authorization URI.
        /// </summary>
        public Uri GetAuthorizationUri()
        {
            return new Uri(
                new Uri(_appSettings.Value.AuthBaseUri), "authorization" +
                "?client_id=" + _appSettings.Value.AuthClientId +
                "&response_type=code" + 
                "&redirect_uri=" + _appSettings.Value.AuthRedirectUri
            );
        }
        
        
        /// <summary>
        /// Determines if the session contains an access token.
        /// </summary>
        public bool IsAuthenticated()
        {
            return (_sessionService.GetAccessToken().Length > 0);
        }
        
        
        /// <summary>
        /// Destroys the access/refresh tokens stored in the session.
        /// </summary>
        public void LogOut()
        {
            _sessionService.ClearTokens();
        }
        
        
        /// <summary>
        /// Encodes a string as Base64.
        /// </summary>
        private static string Base64Encode(string plainText) 
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(bytes);
        }
    }
}