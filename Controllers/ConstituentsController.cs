using System.Net.Http;
using Blackbaud.AuthCodeFlowTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    
    /// <summary>
    /// Contains endpoints that interact with SKY API (constituents).
    /// </summary>
    [Route("api/[controller]")]
    public class ConstituentsController : Controller
    {

        private readonly IConstituentsService _constituentsService;
        
        public ConstituentsController(IConstituentsService constituentsService) 
        {
            _constituentsService = constituentsService;
        }
        
        
        /// <summary>
        /// Returns a paginated list of constituents.
        /// </summary>
        [HttpGet("constituents")]
        public ActionResult GetList()
        {
            return RequireAuth(_constituentsService.GetConstituents());
        }
        
        
        /// <summary>
        /// Returns a constituent record from a provided ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetById(string id)
        {
            return RequireAuth(_constituentsService.GetConstituent(id));
        }
        
        
        /// <summary>
        /// Redirects to Log In if response returns a 401.
        /// </summary>
        private ActionResult RequireAuth(HttpResponseMessage response)
        {
            if ((int) response.StatusCode == 401)
            {
                return RedirectToAction("LogIn", "Authentication");
            }
            string jsonString = response.Content.ReadAsStringAsync().Result;
            return Json(JsonConvert.DeserializeObject<dynamic>(jsonString));
        }
    }
}