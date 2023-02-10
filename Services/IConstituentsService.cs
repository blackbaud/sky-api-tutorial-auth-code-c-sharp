using Blackbaud.AuthCodeFlowTutorial.Models;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    public interface IConstituentsService
    {   
        Task<ConstituentModel> GetConstituentAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<ConstituentModel>> GetConstituentsAsync(CancellationToken cancellationToken);
    }
}