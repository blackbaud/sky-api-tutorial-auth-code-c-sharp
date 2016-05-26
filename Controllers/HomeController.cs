using Microsoft.AspNetCore.Mvc;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    
    /// <summary>
    /// Handles MVC actions and views.
    /// </summary>
    public class HomeController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }
    }
}