using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.FinancialServices;

public interface IFinancialQueries
{
    Task<decimal> GetClientBalanceAsync(int clientId);

    Task<IEnumerable<FinancialTransaction>> GetClientTransactionsAsync(int clientId, int? limit = null);

    Task<decimal> GetClientDebtAsync(int clientId);

    Task<bool> HasSufficientBalanceAsync(int clientId, decimal requiredAmount);

    Task<decimal> GetFullReplenishmentAmountAsync(int clientId);

    Task<IEnumerable<object>> GetAllClientsBalancesAsync();

    Task<IEnumerable<object>> GetClientsWithDebtAsync();

  
}
