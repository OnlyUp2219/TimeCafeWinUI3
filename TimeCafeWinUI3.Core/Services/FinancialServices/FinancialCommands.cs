using TimeCafeWinUI3.Core.Contracts.Services.FinancialServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.FinancialServices;

public class FinancialCommands : IFinancialCommands
{
    private readonly TimeCafeContext _context;

    public FinancialCommands(TimeCafeContext context)
    {
        _context = context;
    }
    public async Task<FinancialTransaction> DepositAsync(int clientId, decimal amount, string comment = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма пополнения должна быть больше 0");

        var transaction = new FinancialTransaction
        {
            ClientId = clientId,
            Amount = amount,
            TransactionTypeId = 1,
            TransactionDate = DateTime.Now,
            Comment = comment
        };

        _context.FinancialTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<FinancialTransaction> DeductAsync(int clientId, decimal amount, int? visitId = null, string comment = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма списания должна быть больше 0");

        var transaction = new FinancialTransaction
        {
            ClientId = clientId,
            Amount = amount,
            TransactionTypeId = 2,
            TransactionDate = DateTime.Now,
            VisitId = visitId,
            Comment = comment
        };

        _context.FinancialTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }
}