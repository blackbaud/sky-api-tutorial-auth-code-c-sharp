using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers {
    
    public class AuthenticationController : Controller {
        
        private readonly IOptions<AppSettings> _settings;
        
        public AuthenticationController(IOptions<AppSettings> settings) 
        {
            _settings = settings;
        }
        
        [HttpGet("~/auth/authenticated")]
        public ActionResult Authenticated() 
        {
            var refreshToken = new Byte[1000];
            bool tokenOK = HttpContext.Session.TryGetValue("refreshToken", out refreshToken);
            return Json(new { authenticated = tokenOK });
        }
        
        [HttpGet("~/auth/callback")]
        public ActionResult Callback() {
            var code = Request.Query["code"];
            FetchTokens(new Dictionary<string, string>(){
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", _settings.Value.AuthRedirectUri }
            });
            return Redirect("/");
        }
        
        [HttpGet("~/auth/login")]
        public ActionResult LogIn() 
        {
            return Redirect(_settings.Value.AuthBaseUri + "authorization" +
                "?client_id=" + _settings.Value.AuthClientId +
                "&response_type=code" + 
                "&redirect_uri=" + _settings.Value.AuthRedirectUri
            );
        }
        
        [HttpGet("~/auth/logout")]
        public ActionResult LogOut() 
        {
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("refreshToken");
            return Redirect("/");
        }
        
        [NonAction]
        public Dictionary<string, string> FetchTokens(Dictionary<string, string> requestBody) 
        {
            using (var client = new HttpClient()) 
            {
                client.BaseAddress = new Uri(_settings.Value.AuthBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", 
                    "Basic " + Base64Encode(_settings.Value.AuthClientId + ":" + _settings.Value.AuthClientSecret));
                
                HttpResponseMessage response = client.PostAsync("token", 
                    new FormUrlEncodedContent(requestBody)).Result;
                    
                response.EnsureSuccessStatusCode();

                var jsonString = response.Content.ReadAsStringAsync().Result;
                var attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                
                HttpContext.Session.Set("token", Encoding.UTF8.GetBytes(attrs["access_token"]));
                HttpContext.Session.Set("refreshToken", Encoding.UTF8.GetBytes(attrs["refresh_token"]));
                
                return attrs;
            }
        }
        
        [NonAction]
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}