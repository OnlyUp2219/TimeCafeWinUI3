using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.FinancialServices;

public interface IFinancialCommands
{
    Task<FinancialTransaction> DepositAsync(int clientId, decimal amount, string comment = null);

    Task<FinancialTransaction> DeductAsync(int clientId, decimal amount, int? visitId = null, string comment = null);
}