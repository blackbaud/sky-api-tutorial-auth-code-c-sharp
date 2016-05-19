using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    [Route("api/[controller]")]
    public class ConstituentsController : Controller
    {
        [HttpGet("{id}")]
        public dynamic Get(string id)
        {
            using (var client = new HttpClient())
            {
                var token = new Byte[700];
                var tokenOK = HttpContext.Session.TryGetValue("token", out token);
                if (tokenOK)
                {
                    client.BaseAddress = new Uri("https://api.sky.blackbaud.com/constituent/constituents/");
                    client.DefaultRequestHeaders.Add("bb-api-subscription-key", "ea65aa631ee349c68839d2589611a0be");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", 
                        "Bearer " + System.Text.Encoding.UTF8.GetString(token));
                    HttpResponseMessage response = client.GetAsync(id).Result;
                    response.EnsureSuccessStatusCode();
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return new { error = "Token invalid." };
                }
            }
        }
    }
}