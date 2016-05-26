using System;
using System.Net.Http;
using Blackbaud.AuthCodeFlowTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers 
{
    
    
    /// <summary>
    /// Contains endpoints that interact with the authorization provider.
    /// </summary>
    [Route("auth")]
    public class AuthenticationController : Controller 
    {
            
        
        private readonly IOptions<AppSettings> AppSettings;
        private readonly IAuthenticationService AuthService;
        
        
        /// <summary>
        /// Initializer for controller.
        /// </summary>
        public AuthenticationController(IOptions<AppSettings> AppSettings, IAuthenticationService AuthService) 
        {
            this.AppSettings = AppSettings;
            this.AuthService = AuthService;
        }
        
        
        /// <summary>
        /// Returns a JSON response determining session's authenticated status.
        /// </summary>
        [HttpGet("authenticated")]
        public ActionResult Authenticated()
        {
            return Json(new { 
                authenticated = AuthService.IsAuthenticated()
            });
        }
                
        
        /// <summary>
        /// Fetches access token (using auth code from request body) and redirects to Home Page.
        /// </summary>
        [HttpGet("callback")]
        public ActionResult Callback()
        {
            string code = Request.Query["code"];
            AuthService.ExchangeCodeForAccessToken(code);
            return Redirect("/");
        }
        
        
        /// <summary>
        /// Redirects user to authorization endpoint.
        /// </summary>
        [HttpGet("login")]
        public ActionResult LogIn()
        {
            Uri address = AuthService.GetAuthorizationUri();
            return Redirect(address.ToString());
        }
        
        
        /// <summary>
        /// Destroys the authenticated session and redirects to Home Page.
        /// </summary>
        [HttpGet("logout")]
        public ActionResult LogOut()
        {
            AuthService.LogOut();
            return Redirect("/");
        }
        
        
        /// <summary>
        /// Deliberately makes a call to the auth provider to refresh access token.
        /// </summary>
        [HttpGet("refresh-token")]
        public ActionResult RefreshToken()
        {
            HttpResponseMessage response = AuthService.RefreshAccessToken();
            string jsonString = response.Content.ReadAsStringAsync().Result;
            return Json(JsonConvert.DeserializeObject<dynamic>(jsonString));
        }
    }
}