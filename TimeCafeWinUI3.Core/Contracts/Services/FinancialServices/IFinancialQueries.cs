using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.Core.Services.FinancialServices;

namespace TimeCafeWinUI3.Core.Contracts.Services.FinancialServices;

public interface IFinancialQueries
{
    Task<decimal> GetClientBalanceAsync(int clientId);

    Task<IEnumerable<FinancialTransaction>> GetClientTransactionsAsync(int clientId, int? limit = null);

    Task<decimal> GetClientDebtAsync(int clientId);

    Task<bool> HasSufficientBalanceAsync(int clientId, decimal requiredAmount);

    Task<IEnumerable<ClientBalanceDto>> GetAllClientsBalancesAsync();

    Task<IEnumerable<ClientBalanceDto>> GetClientsWithDebtAsync();


}
