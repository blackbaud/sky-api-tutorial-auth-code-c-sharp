using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public class AuthenticationService : IAuthenticationService
    {   
        
        private readonly IOptions<AppSettings> AppSettings;
        private readonly HttpContext Context;

        
        public AuthenticationService(IOptions<AppSettings> AppSettings, IHttpContextAccessor contextAccessor)
        {
            this.AppSettings = AppSettings;
            Context = contextAccessor.HttpContext;
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
                string url = new Uri(
                    new Uri(AppSettings.Value.AuthBaseUri), "token").ToString();
                
                // Set request headers.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization", "Basic " + Base64Encode(AppSettings.Value.AuthClientId + ":" + AppSettings.Value.AuthClientSecret));
                
                // Fetch tokens from auth server.
                HttpResponseMessage response = client.PostAsync(url, 
                    new FormUrlEncodedContent(requestBody)).Result;
                
                // Save the access/refresh tokens in the Session.
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, string> attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                    Context.Session.SetString("token", attrs["access_token"]);
                    Context.Session.SetString("refreshToken", attrs["refresh_token"]);
                }
                
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
                { "redirect_uri", AppSettings.Value.AuthRedirectUri }
            });
        }
        
        
        /// <summary>
        /// Refreshes the expired access token (from the refresh token stored in the session).
        /// </summary>
        public HttpResponseMessage RefreshAccessToken()
        {
            return FetchTokens(new Dictionary<string, string>(){
                { "grant_type", "refresh_token" },
                { "refresh_token", Context.Session.GetString("refreshToken") }
            });
        }
        
        
        /// <summary>
        /// Builds and returns a string representative of the provider's authorization URI.
        /// </summary>
        public Uri GetAuthorizationUri()
        {
            return new Uri(
                new Uri(AppSettings.Value.AuthBaseUri), "authorization" +
                "?client_id=" + AppSettings.Value.AuthClientId +
                "&response_type=code" + 
                "&redirect_uri=" + AppSettings.Value.AuthRedirectUri
            );
        }
        
        
        /// <summary>
        /// Determines if the session contains an access token.
        /// </summary>
        public bool IsAuthenticated()
        {
            try
            {
                byte[] token = new Byte[1];
                return Context.Session.TryGetValue("token", out token);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            
        }
        
        
        /// <summary>
        /// Destroys the access/refresh tokens stored in the session.
        /// </summary>
        public void LogOut()
        {
            Context.Session.Remove("token");
            Context.Session.Remove("refreshToken");
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