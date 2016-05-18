using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Flurl;
using Flurl.Http;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers {
    public class AuthenticationController : Controller {
        
        [HttpGet("~/auth/authenticated")]
        public ActionResult Authenticated() 
        {
            return Json(new { authenticated = false });
        }
        
        [HttpGet("~/auth/callback")]
        public ActionResult Callback() {
            
            var responseString = "https://oauth2.sky.blackbaud.com/token"
                .WithHeader("Authorization", "Basic " + Base64Encode("5079f009-4b13-4699-945c-3b584b265694" + ":" + "pPS8SS0dliufKJISqlgh3Cu7QTUCqdJfVUK95qiIZiU="))
                .WithHeader("Accept", "application/x-www-form-urlencoded")
                .PostUrlEncodedAsync(new { 
                    grant_type = "authorization_code",
                    code = Request.Query["code"],
                    redirect_uri = "https://localhost:5000/auth/callback"
                 })
                .ReceiveString();
            
            string result = responseString.Result;
            
            return Content("Hi there! " + result);
        }
        
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        
        // public static async Task<string> GetToken() {
        //     using (var client = new HttpClient()) 
        //     {
        //         var data = "";
        //         var values = new StringContent(data);
        //         var response = await client.PostAsync("https://oauth2.sky.blackbaud.com/token", values);
        //         var responseString = await response.Content.ReadAsStringAsync();
        //         return responseString.ToString();
        //     }
        // }
        
        [HttpGet("~/auth/login")]
        public ActionResult Login() 
        {
            return Redirect("https://oauth2.sky.blackbaud.com/authorization?" +
                "client_id=" + "5079f009-4b13-4699-945c-3b584b265694" +
                "&response_type=code" + 
                "&redirect_uri=" + "https://localhost:5000/auth/callback"
            );
        }
    }
}