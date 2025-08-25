using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services.ClientServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientServices;

public class ClientQueries : IClientQueries
{
    private readonly Dictionary<int, bool> _confirmedPhones = new();
    private readonly TimeCafeContext _context;

    public ClientQueries(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Client> Items, int TotalCount)> GetClientsPageAsync(int pageNumber, int pageSize)
    {
        var items = await _context.Clients
            .AsNoTracking()
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        var totalCount = await _context.Clients.CountAsync();

        return (items, totalCount);
    }

    public async Task<Client> GetClientByIdAsync(int clientId)
    {
        return await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task<IEnumerable<ClientStatus>> GetClientStatusesAsync()
    {
        return await _context.ClientStatuses.ToListAsync();
    }

    public async Task<IEnumerable<Gender>> GetGendersAsync()
    {
        return await _context.Genders.ToListAsync();
    }

    public async Task<bool> IsPhoneConfirmedAsync(int clientId)
    {
        if (_confirmedPhones.TryGetValue(clientId, out var confirmed))
            return confirmed;

        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        var isConfirmed = client.StatusId == (int)ClientStatusType.Active;
        _confirmedPhones[clientId] = isConfirmed;
        return isConfirmed;
    }
}