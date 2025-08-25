using TimeCafeWinUI3.Core.Contracts.Services.TariffServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.TariffServices;

public class TariffCommands : ITariffCommands
{
    private readonly TimeCafeContext _context;

    public TariffCommands(TimeCafeContext context)
    {
        _context = context;
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
