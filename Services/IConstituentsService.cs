using System.Net.Http;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IConstituentsService
    {   
        HttpResponseMessage GetConstituent(string endpoint);
        HttpResponseMessage GetConstituents();
    }
}