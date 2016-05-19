using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers {
    
    
    public class AuthenticationController : Controller {
        
        
        [HttpGet("~/auth/authenticated")]
        public ActionResult Authenticated() 
        {
            return Json(new { authenticated = false });
        }
        
        
        [HttpGet("~/auth/callback")]
        public ActionResult Callback() {
            string token = FetchToken(Request.Query["code"]);
            //return Content("Hi there! " + token);
            HttpContext.Session.Set("token", Encoding.UTF8.GetBytes(token));
            return RedirectToAction("Home");
        }
        
        
        [HttpGet("~/auth/login")]
        public ActionResult Login() 
        {
            return Redirect("https://oauth2.sky.blackbaud.com/authorization?" +
                "client_id=" + "5079f009-4b13-4699-945c-3b584b265694" +
                "&response_type=code" + 
                "&redirect_uri=" + "https://localhost:5000/auth/callback"
            );
        }
        
        static string FetchToken(string code) 
        {
            using (var client = new HttpClient()) 
            {
                client.BaseAddress = new Uri("https://oauth2.sky.blackbaud.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + Base64Encode("5079f009-4b13-4699-945c-3b584b265694" + ":" + "pPS8SS0dliufKJISqlgh3Cu7QTUCqdJfVUK95qiIZiU="));
                
                var body = new Dictionary<string, string>(){
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", "https://localhost:5000/auth/callback" }
                };
                
                HttpResponseMessage response = client.PostAsync("token", 
                    new FormUrlEncodedContent(body)).Result;
                    
                response.EnsureSuccessStatusCode();
                
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var attrs = JsonConvert.DeserializeObject<dynamic>(jsonString);
                
                
                return attrs.access_token;
            }
        }
        
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}