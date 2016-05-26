using System;
using System.Net.Http;
using Blackbaud.AuthCodeFlowTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers 
{
    
    /// <summary>
    /// Contains endpoints that interact with the authorization provider.
    /// </summary>
    [Route("auth")]
    public class AuthenticationController : Controller 
    {
            
        private readonly IAuthenticationService _authService;
        
        public AuthenticationController(IAuthenticationService authService) 
        {
            _authService = authService;
        }
        
        
        /// <summary>
        /// Returns a JSON response determining session's authenticated status.
        /// </summary>
        [HttpGet("authenticated")]
        public ActionResult Authenticated()
        {
            return Json(new { 
                authenticated = _authService.IsAuthenticated()
            });
        }
                
        
        /// <summary>
        /// Fetches access token (using auth code from request body) and redirects to Home Page.
        /// </summary>
        [HttpGet("callback")]
        public ActionResult Callback()
        {
            string code = Request.Query["code"];
            _authService.ExchangeCodeForAccessToken(code);
            return Redirect("/");
        }
        
        
        /// <summary>
        /// Redirects user to authorization endpoint.
        /// </summary>
        [HttpGet("login")]
        public ActionResult LogIn()
        {
            Uri address = _authService.GetAuthorizationUri();
            return Redirect(address.ToString());
        }
        
        
        /// <summary>
        /// Destroys the authenticated session and redirects to Home Page.
        /// </summary>
        [HttpGet("logout")]
        public ActionResult LogOut()
        {
            _authService.LogOut();
            return Redirect("/");
        }
        
        
        /// <summary>
        /// Deliberately makes a call to the auth provider to refresh access token.
        /// </summary>
        [HttpGet("refresh-token")]
        public ActionResult RefreshToken()
        {
            HttpResponseMessage response = _authService.RefreshAccessToken();
            string jsonString = response.Content.ReadAsStringAsync().Result;
            return Json(JsonConvert.DeserializeObject<dynamic>(jsonString));
        }
    }
}