namespace TimeCafeWinUI3.Core.Services.FinancialServices;

public record class ClientBalanceDto(
    int ClientId,
    string FullName,
    string PhoneNumber,
    decimal Balance,
    decimal Debt,
    DateTime LastTransactionDate,
    bool IsActive);