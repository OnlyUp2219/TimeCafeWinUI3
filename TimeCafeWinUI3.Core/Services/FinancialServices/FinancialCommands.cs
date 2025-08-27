using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.FinancialServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.FinancialServices;

public class FinancialCommands : IFinancialCommands
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<FinancialCommands> _logger;
    public FinancialCommands(TimeCafeContext context, IDistributedCache cache, ILogger<FinancialCommands> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
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

        await CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.FinancialTransaction_All,
            CacheKeys.FinancialTransaction_ById(transaction.TransactionId),
            CacheKeys.FinancialTransaction_ByClientId(clientId));

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

        await CacheHelper.RemoveKeysAsync(
          _cache,
          _logger,
          CacheKeys.FinancialTransaction_All,
          CacheKeys.FinancialTransaction_ById(transaction.TransactionId),
          CacheKeys.FinancialTransaction_ByClientId(clientId));

        return transaction;
    }
}