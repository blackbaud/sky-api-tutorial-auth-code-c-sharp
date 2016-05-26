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
        private readonly IConstituentsService _constituentsService;
        private readonly ISessionService _sessionService;
        
        
        /// <summary>
        /// 
        /// </summary>
        public ConstituentsController(IOptions<AppSettings> appSettings, IAuthenticationService authService, ISessionService sessionService, IConstituentsService constituentsService) 
        {
            _appSettings = appSettings;
            _authService = authService;
            _sessionService = sessionService;
            _constituentsService = constituentsService;
        }
        
        
        /// <summary>
        /// Returns a paginated list of constituents.
        /// </summary>
        [HttpGet("constituents")]
        public ActionResult GetList()
        {
            HttpResponseMessage response = _constituentsService.GetConstituents();
            if (response.IsSuccessStatusCode)
            {
                string jsonString = response.Content.ReadAsStringAsync().Result;
                return Json(JsonConvert.DeserializeObject<dynamic>(jsonString));
            }
            else
            {
                return RedirectToAction("LogIn", "Authentication");
            }
        }
        
        
        /// <summary>
        /// Returns a constituent record from a provided ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetById(string id)
        {
            HttpResponseMessage response = _constituentsService.GetConstituent(id);
            if (response.IsSuccessStatusCode) 
            {
                string jsonString = response.Content.ReadAsStringAsync().Result;
                return Json(JsonConvert.DeserializeObject<dynamic>(jsonString));
            }
            else
            {
                return RedirectToAction("LogIn", "Authentication");
            }
        }
    }
}