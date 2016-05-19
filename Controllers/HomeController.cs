using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    public class HomeController : Controller
    {
        //public AppSettings AppSettings { get; }
        
        public HomeController(IOptions<AppSettings> settings) 
        {
            //AppSettings = settings.Value;
        }
        
        public IActionResult Index()
        {
            //ViewData["Message"] = AppSettings.ApplicationTitle.GetType();
            return View();
        }
    }
}