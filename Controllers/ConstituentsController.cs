using System;
using System.Net.Http;
using Blackbaud.AuthCodeFlowTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    
    
    /// <summary>
    /// Contains endpoints that interact with SKY API (constituents).
    /// </summary>
    [Route("api/[controller]")]
    public class ConstituentsController : Controller
    {
        
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IAuthenticationService _authService;
        private readonly ISessionService _sessionService;
        
        
        public ConstituentsController(IOptions<AppSettings> appSettings, IAuthenticationService authService, ISessionService sessionService) 
        {
            _appSettings = appSettings;
            _authService = authService;
            _sessionService = sessionService;
        }
        
        
        /// <summary>
        /// Returns a constituent record from a provided ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string token = _sessionService.GetAccessToken();
                
                if (token.Length > 0)
                {
                    // Set constituent endpoint.
                    client.BaseAddress = new Uri(
                        new Uri(_appSettings.Value.SkyApiBaseUri), "constituent/constituents/");
                    
                    // Set request headers.
                    client.DefaultRequestHeaders.Add(
                        "bb-api-subscription-key", _appSettings.Value.AuthSubscriptionKey);
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "Authorization", "Bearer " + token);
                    
                    // Make the request to constituent API.
                    HttpResponseMessage response = client.GetAsync(id).Result;
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    int statusCode = (int) response.StatusCode;
                    
                    // The access token has expired or is invalid. Request a new one and try again.
                    if (statusCode == 401)
                    {
                        HttpResponseMessage tokenResponse = _authService.RefreshAccessToken();
                        
                        // The token was refreshed successfully.
                        // Attempt to access the API again.
                        if (tokenResponse.IsSuccessStatusCode) 
                        {
                            response = client.GetAsync(id).Result;
                            jsonString = response.Content.ReadAsStringAsync().Result;
                        }
                        
                        // The token cannot be refreshed.
                        // Redirect to the auth login page.
                        else
                        {
                            return RedirectToAction("LogIn", "Authentication");
                        }
                    }

                    return Json(JsonConvert.DeserializeObject<dynamic>(jsonString));
                }
                else
                {
                    return RedirectToAction("LogIn", "Authentication");
                }
            }
        }
    }
}