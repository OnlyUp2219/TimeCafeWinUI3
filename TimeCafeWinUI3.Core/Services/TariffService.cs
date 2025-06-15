using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class TariffService : ITariffService
{
    private readonly TimeCafeContext _context;

    public TariffService(TimeCafeContext context)
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

    public async Task<Tariff> CreateTariffAsync(Tariff tariff)
    {
        tariff.CreatedAt = DateTime.Now;
        tariff.LastModified = DateTime.Now;

        _context.Tariffs.Add(tariff);
        await _context.SaveChangesAsync();
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
        return tariff;
    }

    public async Task<bool> DeleteTariffAsync(int tariffId)
    {
        var tariff = await _context.Tariffs.FindAsync(tariffId);
        if (tariff == null)
            return false;

        _context.Tariffs.Remove(tariff);
        await _context.SaveChangesAsync();
        return true;
    }
} 