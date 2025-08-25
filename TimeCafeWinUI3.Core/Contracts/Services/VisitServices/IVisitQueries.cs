using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.VisitServices;

public interface IVisitQueries
{
    Task<IEnumerable<Visit>> GetActiveVisitsAsync();
    Task<Visit> GetActiveVisitByClientAsync(int clientId);
    Task<bool> IsClientActiveAsync(int clientId);
    Task<bool> IsClientAlreadyEnteredAsync(int clientId);

}
