using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blackbaud.AuthCodeFlowTutorial.Services;

namespace Blackbaud.AuthCodeFlowTutorial.Controllers
{
    [Route("api/[controller]")]
    public class ConstituentsController : Controller
    {
        private readonly IOptions<AppSettings> _settings;
        
        public ConstituentsController(IOptions<AppSettings> settings) 
        {
            _settings = settings;
        }
        
        [HttpGet("{id}")]
        public dynamic Get(string id)
        {
            using (var client = new HttpClient())
            {
                var token = new Byte[700];
                var tokenOK = HttpContext.Session.TryGetValue("token", out token);
                if (tokenOK)
                {
                    client.BaseAddress = new Uri(_settings.Value.SkyApiBaseUri + "constituent/constituents/");
                    client.DefaultRequestHeaders.Add("bb-api-subscription-key", _settings.Value.AuthSubscriptionKey);
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