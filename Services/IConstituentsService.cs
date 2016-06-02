using System.Net.Http;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IConstituentsService
    {   
        HttpResponseMessage GetConstituent(string id);
        HttpResponseMessage GetConstituents();
    }
}