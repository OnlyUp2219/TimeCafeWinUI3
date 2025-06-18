using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;

public interface IVisitService
{
    Task<Visit> EnterClientAsync(int clientId, int tariffId);
    Task<Visit> ExitClientAsync(int visitId);
    Task<IEnumerable<Visit>> GetActiveVisitsAsync();
    Task<Visit?> GetActiveVisitByClientAsync(int clientId);
    Task<bool> IsClientActiveAsync(int clientId);
    Task<bool> IsClientAlreadyEnteredAsync(int clientId);
    Task<decimal> CalculateVisitCostAsync(Visit visit);
    Task<TimeSpan> GetVisitDurationAsync(Visit visit);
    
    // TODO: Автоматический выход всех посетителей при закрытии заведения
    // Task ExitAllVisitorsAsync(string reason = "Автоматический выход при закрытии заведения");
} 