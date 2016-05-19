using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    public class SkyApiController : Controller
    {
        [HttpGet, HttpPost]
        public void Constituents(HttpRequestMessage request)
        {
            var content = request.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
        }
    }
}