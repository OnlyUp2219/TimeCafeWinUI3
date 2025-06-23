using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;

public interface IFinancialService
{
    Task<decimal> GetClientBalanceAsync(int clientId);

    Task<FinancialTransaction> DepositAsync(int clientId, decimal amount, string? comment = null);

    Task<FinancialTransaction> DeductAsync(int clientId, decimal amount, int? visitId = null, string? comment = null);

    Task<IEnumerable<FinancialTransaction>> GetClientTransactionsAsync(int clientId, int? limit = null);

    Task<decimal> GetClientDebtAsync(int clientId);

    Task<bool> HasSufficientBalanceAsync(int clientId, decimal requiredAmount);

    Task<decimal> GetFullReplenishmentAmountAsync(int clientId);

    Task<IEnumerable<object>> GetAllClientsBalancesAsync();

    Task<IEnumerable<object>> GetClientsWithDebtAsync();
} 