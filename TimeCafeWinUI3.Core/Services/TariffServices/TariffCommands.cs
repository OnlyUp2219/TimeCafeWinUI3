using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.TariffServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.TariffServices;

public class TariffCommands : ITariffCommands
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TariffCommands> _logger;

    public TariffCommands(TimeCafeContext context, IDistributedCache cache, ILogger<TariffCommands> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Tariff> CreateTariffAsync(Tariff tariff)
    {
        tariff.CreatedAt = DateTime.Now;
        tariff.LastModified = DateTime.Now;

        _context.Tariffs.Add(tariff);
        await _context.SaveChangesAsync();

        var removeAll = CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.Tariff_All);
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.TariffPagesVersion());

        Task.WaitAll(removeAll, removePage);

        return tariff;
    }

    public async Task<Tariff> UpdateTariffAsync(Tariff tariff)
    {
        var existingTariff = await _context.Tariffs.FindAsync(tariff.TariffId);
        if (existingTariff == null)
            throw new KeyNotFoundException($"Тариф с ID {tariff.TariffId} не найден");

        tariff.LastModified = DateTime.Now;
        _context.Entry(existingTariff).CurrentValues.SetValues(tariff);
        await _context.SaveChangesAsync();

        var removeAll = CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.Tariff_All,
            CacheKeys.Tariff_ById(tariff.TariffId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.TariffPagesVersion());

        Task.WaitAll(removeAll, removePage);

        return tariff;
    }

    public async Task<bool> DeleteTariffAsync(int tariffId)
    {
        var tariff = await _context.Tariffs.FindAsync(tariffId);
        if (tariff == null)
            return false;

        _context.Tariffs.Remove(tariff);
        await _context.SaveChangesAsync();

        await CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.Tariff_All,
            CacheKeys.Tariff_ById(tariffId));
        await CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.TariffPagesVersion());

        return true;
    }
}
