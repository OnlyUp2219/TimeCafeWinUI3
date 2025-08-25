using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services.VisitServices;
using TimeCafeWinUI3.Core.Models;


namespace TimeCafeWinUI3.Core.Services.VisitServices;

public class VisitQueries : IVisitQueries
{
    private readonly TimeCafeContext _context;

    public VisitQueries(TimeCafeContext context)
    {
        _context = context;
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

    public async Task<Visit> GetActiveVisitByClientAsync(int clientId)
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
}
