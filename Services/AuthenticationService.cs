using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public class AuthenticationService : IAuthenticationService
    {   
        
        private readonly IOptions<AppSettings> appSettings;
        private readonly HttpContext context;

        
        public AuthenticationService(IOptions<AppSettings> AppSettings, IHttpContextAccessor contextAccessor)
        {
            this.appSettings = AppSettings;
            this.context = contextAccessor.HttpContext;
            Console.WriteLine("Context: ", context);
        }
        
        
        
        /// <summary>
        /// Fetches access/refresh tokens from the provider.
        /// </summary>
        private dynamic FetchTokens(Dictionary<string, string> requestBody) 
        {
            using (HttpClient client = new HttpClient()) 
            {   
                // Build token endpoint URL.
                string url = new Uri(
                    new Uri(appSettings.Value.AuthBaseUri), "token").ToString();
                
                // Set request headers.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization", 
                    "Basic " + Base64Encode(appSettings.Value.AuthClientId + ":" + appSettings.Value.AuthClientSecret));
                
                // Fetch tokens from auth server.
                HttpResponseMessage response = client.PostAsync(url, 
                    new FormUrlEncodedContent(requestBody)).Result;
                
                // Save the access/refresh tokens in the Session.
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    
                    Dictionary<string, string> attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                    context.Session.SetString("token", attrs["access_token"]);
                    context.Session.SetString("refreshToken", attrs["refresh_token"]);
                    
                    return JsonConvert.DeserializeObject<dynamic>(jsonString);
                }
                
                // There was an error with the request.
                else
                {
                    return new {
                        error = response
                    };
                }
            }
        }
        
        
        /// <summary>
        /// Fetches a new set of access/refresh tokens (from an authorization code).
        /// </summary>
        public void GetAccessToken(string code)
        {
            FetchTokens(new Dictionary<string, string>(){
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", appSettings.Value.AuthRedirectUri }
            });
        }
        
        
        /// <summary>
        /// Returns a string representative of the provider's authorization URI.
        /// </summary>
        public Uri GetAuthorizationUri()
        {
            return new Uri(
                new Uri(appSettings.Value.AuthBaseUri),
                "authorization" +
                "?client_id=" + appSettings.Value.AuthClientId +
                "&response_type=code" + 
                "&redirect_uri=" + appSettings.Value.AuthRedirectUri
            );
        }
        
        
        /// <summary>
        /// Fetches a new set of access/refresh tokens (from a refresh token).
        /// </summary>
        public dynamic RefreshTokens()
        {
            byte[] tokenOut = new Byte[1];
            bool refreshTokenOkay = context.Session.TryGetValue("refreshToken", out tokenOut);
            
            if (refreshTokenOkay)
            {
                string token = context.Session.GetString("refreshToken");
                Console.WriteLine("Refresh Token: " + token);
                return FetchTokens(new Dictionary<string, string>(){
                    { "grant_type", "refresh_token" },
                    { "refresh_token", token }
                });
            }
            else
            {
                Console.WriteLine("No refresh token!!!!!!");
                return new {
                    error = "Refresh token does not exist."
                };
            }
        }
        
        
        /// <summary>
        /// Determines if the session contains an access token.
        /// </summary>
        public bool IsAuthenticated()
        {
            try
            {
                byte[] token = new Byte[1];
                return context.Session.TryGetValue("token", out token);
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
            context.Session.Remove("token");
            context.Session.Remove("refreshToken");
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