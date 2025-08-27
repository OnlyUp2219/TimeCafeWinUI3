using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.TariffServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.TariffServices;

public class TariffQueries : ITariffQueries
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TariffCommands> _logger;
    public TariffQueries(TimeCafeContext context, IDistributedCache cache, ILogger<TariffCommands> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Tariff>> GetAllTariffsAsync()
    {
        var cached = await CacheHelper.GetAsync<IEnumerable<Tariff>>(
        _cache,
        _logger,
        CacheKeys.Tariff_All);
        if (cached != null)
            return cached;

        var entity = await _context.Tariffs
        .Include(t => t.BillingType)
        .Include(t => t.Theme)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

        await CacheHelper.SetAsync(
        _cache,
        _logger,
        CacheKeys.Tariff_All,
        entity);

        return entity;
    }

    public async Task<IEnumerable<Tariff>> GetTariffsPageAsync(int pageNumber, int pageSize)
    {
        string versionStr = await _cache.GetStringAsync(CacheKeys.TariffPagesVersion());
        int version = int.TryParse(versionStr, out var v) ? v : 1;

        var cacheKey = CacheKeys.Tariff_Page(pageNumber, version);

        var cached = await CacheHelper.GetAsync<IEnumerable<Tariff>>(
            _cache,
            _logger,
            cacheKey);
        if (cached != null)
            return cached;

        var items = await _context.Tariffs
        .AsNoTracking()
        .Include(t => t.BillingType)
        .Include(t => t.Theme)
        .OrderByDescending(t => t.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync()
        .ConfigureAwait(false);

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            cacheKey,
            items,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return (items);
    }

    public async Task<int> GetTotalPageAsync()
    {
        return await _context.Tariffs.CountAsync();
    }

    public async Task<Tariff> GetTariffByIdAsync(int tariffId)
    {
        var cached = await CacheHelper.GetAsync<Tariff>(
        _cache,
        _logger,
        CacheKeys.Tariff_ById(tariffId));
        if (cached != null)
            return cached;

        var entity = await _context.Tariffs
        .Include(t => t.BillingType)
        .Include(t => t.Theme)
        .FirstOrDefaultAsync(t => t.TariffId == tariffId);

        await CacheHelper.SetAsync(
        _cache,
        _logger,
        CacheKeys.Tariff_ById(tariffId),
        entity);

        return entity;
    }
}