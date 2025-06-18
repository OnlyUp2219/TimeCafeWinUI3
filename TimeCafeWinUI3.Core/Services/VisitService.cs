using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class VisitService : IVisitService
{
    private readonly TimeCafeContext _context;
    private readonly IBillingTypeService _billingTypeService;

    public VisitService(TimeCafeContext context, IBillingTypeService billingTypeService)
    {
        _context = context;
        _billingTypeService = billingTypeService;
    }

    public async Task<Visit> EnterClientAsync(int clientId, int tariffId)
    {
        // Проверяем, что клиент активен
        if (!await IsClientActiveAsync(clientId))
            throw new InvalidOperationException("Клиент не имеет активного статуса");

        // Проверяем, что клиент еще не вошел
        if (await IsClientAlreadyEnteredAsync(clientId))
            throw new InvalidOperationException("Ошибка. Вход уже осуществлен");

        var tariff = await _context.Tariffs
            .Include(t => t.BillingType)
            .FirstOrDefaultAsync(t => t.TariffId == tariffId);

        if (tariff == null)
            throw new KeyNotFoundException($"Тариф с ID {tariffId} не найден");

        if (tariff.BillingType == null)
            throw new InvalidOperationException($"У тарифа {tariff.TariffName} не указан тип тарификации");

        var visit = new Visit
        {
            ClientId = clientId,
            TariffId = tariffId,
            BillingTypeId = tariff.BillingTypeId,
            EntryTime = DateTime.Now,
            ExitTime = null,
            VisitCost = null
        };

        _context.Visits.Add(visit);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка сохранения Visit: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            throw new InvalidOperationException($"Ошибка создания посещения: {ex.Message}", ex);
        }

        return visit;
    }

    public async Task<Visit> ExitClientAsync(int visitId)
    {
        var visit = await _context.Visits
            .Include(v => v.Tariff)
            .Include(v => v.BillingType)
            .Include(v => v.Client)
            .FirstOrDefaultAsync(v => v.VisitId == visitId);

        if (visit == null)
            throw new KeyNotFoundException($"Посещение с ID {visitId} не найдено");

        if (visit.ExitTime.HasValue)
            throw new InvalidOperationException("Ошибка. Выход уже осуществлен");

        visit.ExitTime = DateTime.Now;
        visit.VisitCost = await CalculateVisitCostAsync(visit);

        // TODO: Снятие с баланса клиента (депозита)
        // await _financialService.DeductFromBalanceAsync(visit.ClientId, visit.VisitCost.Value);

        await _context.SaveChangesAsync();
        return visit;
    }

    public async Task<IEnumerable<Visit>> GetActiveVisitsAsync()
    {
        return await _context.Visits
            .Include(v => v.Client)
            .Include(v => v.Tariff)
            .Include(v => v.BillingType)
            .Where(v => v.ExitTime == null)
            .OrderByDescending(v => v.EntryTime)
            .ToListAsync();
    }

    public async Task<Visit?> GetActiveVisitByClientAsync(int clientId)
    {
        return await _context.Visits
            .Include(v => v.Tariff)
            .Include(v => v.BillingType)
            .FirstOrDefaultAsync(v => v.ClientId == clientId && v.ExitTime == null);
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

    public async Task<decimal> CalculateVisitCostAsync(Visit visit)
    {
        if (visit.Tariff == null)
            return 0;

        var duration = await GetVisitDurationAsync(visit);
        
        // Проверяем, что BillingTypeId не null
        if (!visit.BillingTypeId.HasValue)
            return 0;
            
        var billingType = await _billingTypeService.GetBillingTypeByIdAsync(visit.BillingTypeId.Value);

        if (billingType == null)
            return 0;

        switch (billingType.BillingTypeId)
        {
            case 1: // Почасовая тарификация
                // Почасовая тарификация с округлением вверх
                var hours = Math.Ceiling(duration.TotalHours);
                return visit.Tariff.Price * (decimal)hours;

            case 2: // Поминутная тарификация
                // Поминутная тарификация
                var minutes = duration.TotalMinutes;
                return visit.Tariff.Price * (decimal)minutes;

            default:
                return 0;
        }
    }

    public async Task<TimeSpan> GetVisitDurationAsync(Visit visit)
    {
        var endTime = visit.ExitTime ?? DateTime.Now;
        return endTime - visit.EntryTime;
    }
} 