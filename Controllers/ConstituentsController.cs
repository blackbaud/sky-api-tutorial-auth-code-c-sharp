using System;
using System.Net.Http;
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
        
        
        public ConstituentsController(IOptions<AppSettings> AppSettings, IAuthenticationService AuthService) 
        {
            this.AppSettings = AppSettings;
            this.AuthService = AuthService;
        }
        
        
        /// <summary>
        /// Returns a constituent record from a provided ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] token = new Byte[700];
                bool tokenOkay = HttpContext.Session.TryGetValue("token", out token);
                
                if (tokenOkay)
                {
                    // Set constituent endpoint.
                    client.BaseAddress = new Uri(
                        new Uri(AppSettings.Value.SkyApiBaseUri), "constituent/constituents/");
                    
                    // Set request headers.
                    client.DefaultRequestHeaders.Add(
                        "bb-api-subscription-key", AppSettings.Value.AuthSubscriptionKey);
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "Authorization", "Bearer " + System.Text.Encoding.UTF8.GetString(token));
                    
                    // Make the request to constituent API.
                    HttpResponseMessage response = client.GetAsync(id).Result;
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    int statusCode = (int) response.StatusCode;
                    
                    // The access token has expired or is invalid. Request a new one and try again.
                    if (statusCode == 401)
                    {
                        HttpResponseMessage tokenResponse = AuthService.RefreshAccessToken();
                        
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