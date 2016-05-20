using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<AppSettings> _settings;
        
        public HomeController(IOptions<AppSettings> settings) 
        {
            _settings = settings;
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}