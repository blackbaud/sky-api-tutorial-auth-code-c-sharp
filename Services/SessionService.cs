using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    
    /// <summary>
    /// 
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly string _accessTokenName;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _refreshTokenName;
        private ISession _session;
        
        
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _accessTokenName = "token";
            _refreshTokenName = "refreshToken";
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public void ClearTokens()
        {
            try 
            {
                _session.Remove(_accessTokenName);
                _session.Remove(_refreshTokenName);
            }
            catch (Exception error)
            {
                Console.WriteLine("LOGOUT ERROR: " + error.Message);
            }
        }
        
        
        /// <summary>
        /// Return access token, if saved, or an empty string.
        /// </summary>
        public string GetAccessToken()
        {
            return TryGetString(_accessTokenName);
        }
        
        
        /// <summary>
        /// Return refresh token, if saved, or an empty string.
        /// </summary>
        public string GetRefreshToken()
        {
            return TryGetString(_refreshTokenName);
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public void SetTokens(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string jsonString = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                _session.SetString(_accessTokenName, attrs["access_token"]);
                _session.SetString(_refreshTokenName, attrs["refresh_token"]);
            }
        }
        
        
        /// <summary>
        /// Return session value as string, if exists, or an empty string.
        /// </summary>
        private string TryGetString(string name)
        {
            byte[] valueBytes = new Byte[700];
            bool valueOkay = _session.TryGetValue(name, out valueBytes);
            if (valueOkay)
            {
                return System.Text.Encoding.UTF8.GetString(valueBytes);
            }
            else
            {
                return "";
            }
        }
    }
}