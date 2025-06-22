using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;

public interface IFinancialService
{
    /// <summary>
    /// Получить текущий баланс клиента
    /// </summary>
    Task<decimal> GetClientBalanceAsync(int clientId);

    /// <summary>
    /// Пополнить депозит клиента
    /// </summary>
    Task<FinancialTransaction> DepositAsync(int clientId, decimal amount, string? comment = null);

    /// <summary>
    /// Списать средства с баланса клиента
    /// </summary>
    Task<FinancialTransaction> DeductAsync(int clientId, decimal amount, int? visitId = null, string? comment = null);

    /// <summary>
    /// Получить историю транзакций клиента
    /// </summary>
    Task<IEnumerable<FinancialTransaction>> GetClientTransactionsAsync(int clientId, int? limit = null);

    /// <summary>
    /// Получить задолженность клиента (отрицательный баланс)
    /// </summary>
    Task<decimal> GetClientDebtAsync(int clientId);

    /// <summary>
    /// Проверить, достаточно ли средств у клиента для минимального посещения
    /// </summary>
    Task<bool> HasSufficientBalanceAsync(int clientId, decimal requiredAmount);

    /// <summary>
    /// Получить сумму для полного пополнения (покрытие задолженности)
    /// </summary>
    Task<decimal> GetFullReplenishmentAmountAsync(int clientId);

    /// <summary>
    /// Получить всех клиентов с их балансами
    /// </summary>
    Task<IEnumerable<ClientBalanceInfo>> GetAllClientsBalancesAsync();

    /// <summary>
    /// Получить клиентов с задолженностью
    /// </summary>
    Task<IEnumerable<ClientBalanceInfo>> GetClientsWithDebtAsync();
}

/// <summary>
/// Информация о балансе клиента
/// </summary>
public class ClientBalanceInfo
{
    public int ClientId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Balance { get; set; }
    public decimal Debt { get; set; }
    public DateTime LastTransactionDate { get; set; }
    public bool IsActive { get; set; }
} 