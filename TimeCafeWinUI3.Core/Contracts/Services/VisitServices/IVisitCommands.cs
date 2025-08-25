using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.VisitServices;

public interface IVisitCommands
{
    Task<decimal> CalculateVisitCostAsync(Visit visit);
    TimeSpan GetVisitDuration(Visit visit);

    // TODO: Автоматический выход всех посетителей при закрытии заведения
    // Task ExitAllVisitorsAsync(string reason = "Автоматический выход при закрытии заведения");
    Task<Visit> EnterClientAsync(int clientId, int tariffId, int minimumEntryMinutes);
    Task<Visit> ExitClientAsync(int visitId);
}