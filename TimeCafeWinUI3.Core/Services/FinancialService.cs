using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class FinancialService : IFinancialService
{
    private readonly TimeCafeContext _context;

    public FinancialService(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetClientBalanceAsync(int clientId)
    {
        var transactions = await _context.FinancialTransactions
            .Where(t => t.ClientId == clientId)
            .ToListAsync();

        return transactions.Sum(t => t.TransactionTypeId == 1 ? t.Amount : -t.Amount);
    }

    public async Task<FinancialTransaction> DepositAsync(int clientId, decimal amount, string? comment = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма пополнения должна быть больше 0");

        var transaction = new FinancialTransaction
        {
            ClientId = clientId,
            Amount = amount,
            TransactionTypeId = 1,
            TransactionDate = DateTime.Now
        };

        _context.FinancialTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<FinancialTransaction> DeductAsync(int clientId, decimal amount, int? visitId = null, string? comment = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма списания должна быть больше 0");

        var transaction = new FinancialTransaction
        {
            ClientId = clientId,
            Amount = amount,
            TransactionTypeId = 2,
            TransactionDate = DateTime.Now,
            VisitId = visitId
        };

        _context.FinancialTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<IEnumerable<FinancialTransaction>> GetClientTransactionsAsync(int clientId, int? limit = null)
    {
        var query = _context.FinancialTransactions
            .Include(t => t.TransactionType)
            .Include(t => t.Visit)
            .Where(t => t.ClientId == clientId)
            .OrderByDescending(t => t.TransactionDate);

        if (limit.HasValue)
        {
            return await query.Take(limit.Value).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<decimal> GetClientDebtAsync(int clientId)
    {
        var balance = await GetClientBalanceAsync(clientId);
        return balance < 0 ? Math.Abs(balance) : 0;
    }

    public async Task<bool> HasSufficientBalanceAsync(int clientId, decimal requiredAmount)
    {
        var balance = await GetClientBalanceAsync(clientId);
        return balance >= requiredAmount;
    }

    public async Task<decimal> GetFullReplenishmentAmountAsync(int clientId)
    {
        var debt = await GetClientDebtAsync(clientId);
        return debt;
    }

    public async Task<IEnumerable<object>> GetAllClientsBalancesAsync()
    {
        var clients = await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.FinancialTransactions)
            .ToListAsync();

        var result = new List<object>();

        foreach (var client in clients)
        {
            var balance = client.FinancialTransactions.Sum(t => t.TransactionTypeId == 1 ? t.Amount : -t.Amount);
            var lastTransaction = client.FinancialTransactions
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefault();

            dynamic clientInfo = new ExpandoObject();
            clientInfo.ClientId = client.ClientId;
            clientInfo.FullName = $"{client.LastName} {client.FirstName} {client.MiddleName}".Trim();
            clientInfo.PhoneNumber = client.PhoneNumber;
            clientInfo.Balance = balance;
            clientInfo.Debt = balance < 0 ? Math.Abs(balance) : 0;
            clientInfo.LastTransactionDate = lastTransaction?.TransactionDate ?? client.CreatedAt;
            clientInfo.IsActive = client.Status?.StatusName == "Активный";

            result.Add(clientInfo);
        }

        return result.OrderByDescending(c => ((dynamic)c).Debt).ThenBy(c => ((dynamic)c).FullName);
    }

    public async Task<IEnumerable<object>> GetClientsWithDebtAsync()
    {
        var allClients = await GetAllClientsBalancesAsync();
        return allClients.Where(c => ((dynamic)c).Debt > 0);
    }
} 