using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services.TariffServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.TariffServices;

public class TariffQueries : ITariffQueries
{
    private readonly TimeCafeContext _context;

    public TariffQueries(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tariff>> GetAllTariffsAsync()
    {
        return await _context.Tariffs
            .Include(t => t.BillingType)
            .Include(t => t.Theme)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Tariff> Items, int TotalCount)> GetTariffsPageAsync(int pageNumber, int pageSize)
    {
        var items = await _context.Tariffs
            .AsNoTracking()
            .Include(t => t.BillingType)
            .Include(t => t.Theme)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        var totalCount = await _context.Tariffs.CountAsync();

        return (items, totalCount);
    }

    public async Task<Tariff> GetTariffByIdAsync(int tariffId)
    {
        return await _context.Tariffs
            .Include(t => t.BillingType)
            .Include(t => t.Theme)
            .FirstOrDefaultAsync(t => t.TariffId == tariffId);
    }
}