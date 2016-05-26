using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    
    /// <summary>
    /// Interacts directly with SKY API Constituent endpoints.
    /// </summary>
    public class ConstituentsService : IConstituentsService
    {
        
        private Uri _apiBaseUri;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ISessionService _sessionService;
        private readonly IAuthenticationService _authService;
        
        public ConstituentsService(IOptions<AppSettings> appSettings, ISessionService sessionService, IAuthenticationService authService)
        {
            _appSettings = appSettings;
            _sessionService = sessionService;
            _authService = authService;
            _apiBaseUri = new Uri(new Uri(_appSettings.Value.SkyApiBaseUri), "constituent/");
        }
        
        
        /// <summary>
        /// Requests that the auth service refresh the access token and returns true if successful.
        /// </summary>
        private bool TryRefreshToken()
        {
            HttpResponseMessage tokenResponse = _authService.RefreshAccessToken();
            return (tokenResponse.IsSuccessStatusCode);
        }
        
        
        /// <summary>
        /// Performs HTTP requests (POST/GET) and returns the response.
        /// <param name="method" type="String">The HTTP method, post, get</param>
        /// <param name="endpoint" type="String">The API endpoint</param>
        /// <param name="content" type="HttpContent">The request body content</param>
        /// </summary>
        private HttpResponseMessage Proxy(string method, string endpoint, StringContent content = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string token = _sessionService.GetAccessToken();
                HttpResponseMessage response;
                
                // Set constituent endpoint.
                client.BaseAddress = _apiBaseUri;
                
                // Set request headers.
                client.DefaultRequestHeaders.Add("bb-api-subscription-key", _appSettings.Value.AuthSubscriptionKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
                
                // Make the request to constituent API.
                switch (method.ToLower())
                {
                    default:
                    case "get":
                    response = client.GetAsync(endpoint).Result;
                    break;
                    
                    case "post":
                    response = client.PostAsync(endpoint, content).Result;
                    break;
                }
                
                return response;
            }
        }
        
        
        /// <summary>
        /// Returns a response containing a constituent record (from an ID).
        /// </summary>
        public HttpResponseMessage GetConstituent(string id) 
        {
            // Make the request.
            HttpResponseMessage response = Proxy("get", "constituents/" + id);
            
            // Handle bad response.
            if (!response.IsSuccessStatusCode)
            {   
                int statusCode = (int) response.StatusCode;
                switch (statusCode)
                {
                    // ID formatted incorrectly.
                    case 400:
                    response.Content = new StringContent("{ error: \"The specified constituent ID was not in the correct format.\" }");
                    break;
                    
                    // Token expired. Refresh the token and try again.
                    case 401:
                    bool tokenRefreshed = TryRefreshToken();
                    if (tokenRefreshed)
                    {
                        response = Proxy("get", "constituents/" + id);
                    }
                    break;
                    
                    // Constituent not found.
                    case 404:
                    response.Content = new StringContent("{ error: \"No constituent record was found with the specified ID.\" }");
                    break;
                }
            }
            
            return response;
        }
        
        
        /// <summary>
        /// Returns a response containing a paginated list of constituents.
        /// </summary>
        public HttpResponseMessage GetConstituents() 
        {
            return Proxy("get", "constituents/");
        }
    }
}