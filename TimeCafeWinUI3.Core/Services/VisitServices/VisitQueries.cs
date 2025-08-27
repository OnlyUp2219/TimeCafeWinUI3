using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.VisitServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;


namespace TimeCafeWinUI3.Core.Services.VisitServices;

public class VisitQueries : IVisitQueries
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<VisitQueries> _logger;
    public VisitQueries(TimeCafeContext context, IDistributedCache cache, ILogger<VisitQueries> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Visit>> GetActiveVisitsAsync()
    {
        var cached = await CacheHelper.GetAsync<IEnumerable<Visit>>(
            _cache,
            _logger,
            CacheKeys.Visit_All);
        if (cached != null)
            return cached;

        var entity = await _context.Visits
           .Include(v => v.Client)
           .Include(v => v.Tariff)
           .Include(v => v.BillingType)
           .Where(v => v.ExitTime == null)
           .OrderByDescending(v => v.EntryTime)
           .ToListAsync();

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.Visit_All,
            entity);

        return entity;
    }

    public async Task<Visit> GetActiveVisitByClientAsync(int clientId)
    {
        var cached = await CacheHelper.GetAsync<Visit>(
            _cache,
            _logger,
            CacheKeys.Visit_ByCliendId(clientId));
        if (cached != null)
            return cached;

        var entity = await _context.Visits
            .Include(v => v.Tariff)
            .Include(v => v.BillingType)
            .FirstOrDefaultAsync(v => v.ClientId == clientId && v.ExitTime == null);

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.Visit_ByCliendId(clientId),
            entity);

        return entity;
    }

    public async Task<bool> IsClientActiveAsync(int clientId)
    {
        var client = await _context.Clients
            .Include(c => c.Status)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

        return client?.Status?.StatusName == "Активный";
    }

    public async Task<bool> IsClientAlreadyEnteredAsync(int clientId)
    {
        return await _context.Visits
            .AnyAsync(v => v.ClientId == clientId && v.ExitTime == null);
    }
}
