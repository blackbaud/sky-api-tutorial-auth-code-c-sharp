using System;
using System.Net.Http;
using Blackbaud.AuthCodeFlowTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IConstituentsService
    {   
        HttpResponseMessage GetConstituent(string endpoint);
        HttpResponseMessage GetConstituents();
    }
}