using System;
using Microsoft.CSharp.RuntimeBinder;
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
        [HttpGet("~/auth/authenticated")]
        public ActionResult Authenticated()
        {
            return Json(new { 
                authenticated = AuthService.IsAuthenticated()
            });
        }
        
        
        
        /// <summary>
        /// Fetches auth tokens (using auth code from request body) and redirects to Home Page.
        /// </summary>
        [HttpGet("~/auth/callback")]
        public ActionResult Callback() 
        {
            string code = Request.Query["code"];
            AuthService.GetAccessToken(code);
            return Redirect("/");
        }
        
        
        
        /// <summary>
        /// Redirects user to authorization endpoint.
        /// </summary>
        [HttpGet("~/auth/login")]
        public ActionResult LogIn()
        {
            Uri address = AuthService.GetAuthorizationUri();
            return Redirect(address.ToString());
        }
        
        
        
        /// <summary>
        /// Destroys the authenticated session and redirects to Home Page.
        /// </summary>
        [HttpGet("~/auth/logout")]
        public ActionResult LogOut()
        {
            AuthService.LogOut();
            return Redirect("/");
        }
        
        
        
        /// <summary>
        /// 
        /// </summary>
        [HttpGet("~/auth/refresh-token")]
        public ActionResult RefreshToken()
        {
            dynamic result = AuthService.RefreshTokens();
            Console.WriteLine("Refresh Token/ Result: ", result);
            try
            {
                string error = result.error;
                return RedirectToAction("LogIn");
            }
            catch (RuntimeBinderException)
            {
                return Json(result);
            } 
        }
    }
}