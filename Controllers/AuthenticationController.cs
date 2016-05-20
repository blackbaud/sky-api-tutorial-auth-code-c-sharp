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
            var token = new Byte[1];
            bool tokenOK = HttpContext.Session.TryGetValue("token", out token);
            return Json(new { authenticated = tokenOK });
        }
        
        [HttpGet("~/auth/callback")]
        public ActionResult Callback() {
            string token = FetchToken(Request.Query["code"]);
            HttpContext.Session.Set("token", Encoding.UTF8.GetBytes(token));
            return Redirect("/");
        }
        
        [HttpGet("~/auth/login")]
        public ActionResult Login() 
        {
            return Redirect(_settings.Value.AuthBaseUri + "authorization" +
                "?client_id=" + _settings.Value.AuthClientId +
                "&response_type=code" + 
                "&redirect_uri=" + _settings.Value.AuthRedirectUri
            );
        }
        
        [NonAction]
        public string FetchToken(string code) 
        {
            using (var client = new HttpClient()) 
            {
                client.BaseAddress = new Uri(_settings.Value.AuthBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", 
                    "Basic " + Base64Encode(_settings.Value.AuthClientId + ":" + _settings.Value.AuthClientSecret));
                
                var body = new Dictionary<string, string>(){
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", _settings.Value.AuthRedirectUri }
                };
                
                HttpResponseMessage response = client.PostAsync("token", 
                    new FormUrlEncodedContent(body)).Result;
                    
                response.EnsureSuccessStatusCode();

                var jsonString = response.Content.ReadAsStringAsync().Result;
                var attrs = JsonConvert.DeserializeObject<dynamic>(jsonString);
                
                return attrs.access_token;
            }
        }
        
        [NonAction]
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}