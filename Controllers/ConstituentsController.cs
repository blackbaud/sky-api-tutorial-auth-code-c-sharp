using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    
    [Route("api/[controller]")]
    public class ConstituentsController : Controller
    {
        
        
        
        private readonly IOptions<AppSettings> AppSettings;
        private readonly IAuthenticationService AuthService;
        
        
        
        /// <summary>
        /// </summary>
        public ConstituentsController(IOptions<AppSettings> AppSettings, IAuthenticationService AuthService) 
        {
            this.AppSettings = AppSettings;
            this.AuthService = AuthService;
        }
        
        
        
        /// <summary>
        /// </summary>
        [HttpGet("{id}")]
        public dynamic Get(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] token = new Byte[700];
                //byte[] refreshToken = new Byte[700];
                bool tokenOkay = HttpContext.Session.TryGetValue("token", out token);
                //bool refreshTokenOkay = HttpContext.Session.TryGetValue("refreshtoken", out refreshToken);
                
                if (tokenOkay)
                {
                    // Constituent endpoint
                    client.BaseAddress = new Uri(
                        new Uri(AppSettings.Value.SkyApiBaseUri),
                        "constituent/constituents/");
                    
                    // Set request headers.
                    client.DefaultRequestHeaders.Add(
                        "bb-api-subscription-key", 
                        AppSettings.Value.AuthSubscriptionKey);
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "Authorization", 
                        "Bearer " + System.Text.Encoding.UTF8.GetString(token));
                    
                    // Make the request to constituent API.
                    HttpResponseMessage response = client.GetAsync(id).Result;
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    dynamic result = new {};
                    
                    Console.WriteLine("RESPONSE: " + jsonString);
                    Console.WriteLine("RESPONSE CODE: " + (int) response.StatusCode);
                    
                    int statusCode = (int) response.StatusCode;
                    switch (statusCode)
                    {
                        // API Call is successful.
                        case 200:
                        break;
                        
                        // Constituent ID is not valid type.
                        case 400:
                        Console.WriteLine("[ERROR] Constituent ID, " + id + ", is not in the appropriate format.");
                        break;
                        
                        // Token has expired or is invalid. Request a new one.
                        case 401:
                        //AuthService.RefreshTokens();
                        Console.WriteLine("[ERROR] Token has expired.");
                        break;
                        
                        // Constituent record not found.
                        case 404:
                        Console.WriteLine("[ERROR] Constituent record not found.");
                        break;
                    }
                        
                    return JsonConvert.DeserializeObject<dynamic>(jsonString); 
                        
                        // var attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                        // AuthenticationController.FetchTokens(new Dictionary<string, string>(){
                        //     { "grant_type", "refresh_token" },
                        //     { "refresh_token", System.Text.Encoding.UTF8.GetString(refreshToken) }
                        // });
                    
                    
                }
                else
                {
                    return new { 
                        error = "Authentication token is invalid or could not found." 
                    };
                }
            }
        }
    }
}