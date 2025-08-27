using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.FinancialServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.FinancialServices;

public class FinancialQueries : IFinancialQueries
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<FinancialQueries> _logger;

    public FinancialQueries(TimeCafeContext context, IDistributedCache cache, ILogger<FinancialQueries> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<decimal> GetClientBalanceAsync(int clientId)
    {
        var cached = await CacheHelper.GetAsync<decimal?>(
            _cache,
            _logger,
            CacheKeys.FinancialTransaction_ByClientId(clientId));
        if (cached.HasValue)
            return cached.Value;

        var transactions = await _context.FinancialTransactions
            .Where(t => t.ClientId == clientId)
            .ToListAsync();

        var balance = transactions.Sum(t => t.TransactionTypeId == 1 ? t.Amount : -t.Amount);

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.FinancialTransaction_ByClientId(clientId),
            balance);

        return balance;
    }

    public async Task<IEnumerable<FinancialTransaction>> GetClientTransactionsAsync(int clientId, int? limit = null)
    {
        var cached = await CacheHelper.GetAsync<IEnumerable<FinancialTransaction>>(
            _cache,
            _logger,
            CacheKeys.FinancialTransaction_ByClientId(clientId));
        if (cached != null)
            return cached;

        var query = _context.FinancialTransactions
            .Include(t => t.TransactionType)
            .Include(t => t.Visit)
            .Where(t => t.ClientId == clientId)
            .OrderByDescending(t => t.TransactionDate);

        if (limit.HasValue)
        {
            var clientTransLimit = await query.Take(limit.Value).ToListAsync();
            //TODO : Решить, что делать с кэшированием 
            return clientTransLimit;
        }

        var clientTrans = await query.ToListAsync();

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.FinancialTransaction_ByClientId(clientId),
            clientTrans);

        return clientTrans;
    }

    public async Task<decimal> GetClientDebtAsync(int clientId)
    {
        var balance = await GetClientBalanceAsync(clientId);
        return balance < 0 ? Math.Abs(balance) : 0; ;
    }

    public async Task<bool> HasSufficientBalanceAsync(int clientId, decimal requiredAmount)
    {
        var balance = await GetClientBalanceAsync(clientId);
        return balance >= requiredAmount;
    }

    //public async Task<IEnumerable<object>> GetAllClientsBalancesAsync()
    //{
    //    var clients = await _context.Clients
    //        .Include(c => c.Status)
    //        .Include(c => c.FinancialTransactions)
    //        .ToListAsync();

    //    var result = new List<object>();

    //    foreach (var client in clients)
    //    {
    //        var balance = client.FinancialTransactions.Sum(t => t.TransactionTypeId == 1 ? t.Amount : -t.Amount);
    //        var lastTransaction = client.FinancialTransactions
    //            .OrderByDescending(t => t.TransactionDate)
    //            .FirstOrDefault();

    //        dynamic clientInfo = new ExpandoObject();
    //        clientInfo.ClientId = client.ClientId;
    //        clientInfo.FullName = $"{client.LastName} {client.FirstName} {client.MiddleName}".Trim();
    //        clientInfo.PhoneNumber = client.PhoneNumber;
    //        clientInfo.Balance = balance;
    //        clientInfo.Debt = balance < 0 ? Math.Abs(balance) : 0;
    //        clientInfo.LastTransactionDate = lastTransaction?.TransactionDate ?? client.CreatedAt;
    //        clientInfo.IsActive = client.Status?.StatusName == "Активный";

    //        result.Add(clientInfo);
    //    }

    //    return result.OrderByDescending(c => ((dynamic)c).Debt).ThenBy(c => ((dynamic)c).FullName);
    //}

    public async Task<IEnumerable<ClientBalanceDto>> GetAllClientsBalancesAsync()
    {
        var clients = await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.FinancialTransactions)
            .ToListAsync();

        var result = clients.Select(client =>
        {
            var balance = client.FinancialTransactions.Sum(t =>
                t.TransactionTypeId == 1 ? t.Amount : -t.Amount);

            var lastTransactionDate = client.FinancialTransactions
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => t.TransactionDate)
                .FirstOrDefault();

            return new ClientBalanceDto(
                client.ClientId,
                $"{client.LastName} {client.FirstName} {client.MiddleName}".Trim(),
                client.PhoneNumber,
                balance,
                balance < 0 ? Math.Abs(balance) : 0,
                lastTransactionDate == default ? client.CreatedAt : lastTransactionDate,
                client.Status?.StatusName == "Активный"
            );
        });

        return result
            .OrderByDescending(c => c.Debt)
            .ThenBy(c => c.FullName)
            .ToList();
    }

    public async Task<IEnumerable<ClientBalanceDto>> GetClientsWithDebtAsync()
    {
        var allClients = await GetAllClientsBalancesAsync();
        return allClients.Where(c => c.Debt > 0);
    }

}
