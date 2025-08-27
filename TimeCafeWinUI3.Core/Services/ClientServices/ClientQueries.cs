using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.ClientServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientServices;

public class ClientQueries : IClientQueries
{
    private readonly Dictionary<int, bool> _confirmedPhones = new();
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ClientQueries> _logger;

    public ClientQueries(TimeCafeContext context, IDistributedCache cache, ILogger<ClientQueries> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        var cached = await CacheHelper.GetAsync<IEnumerable<Client>>(
            _cache,
            _logger,
            CacheKeys.Client_All);
        if (cached != null)
            return cached;

        var entity = await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        await CacheHelper.SetAsync<IEnumerable<Client>>(
            _cache,
            _logger,
            CacheKeys.Client_All,
            entity);

        return entity;
    }

    public async Task<IEnumerable<Client>> GetClientsPageAsync(int pageNumber, int pageSize)
    {
        var versionStr = await _cache.GetStringAsync(CacheKeys.ClientPagesVersion());
        var version = int.TryParse(versionStr, out var v) ? v : 1;

        var cacheKey = CacheKeys.Client_Page(pageNumber, version);

        var cached = await CacheHelper.GetAsync<IEnumerable<Client>>(
            _cache,
            _logger,
            cacheKey);
        if (cached != null) return cached;

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

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            cacheKey,
            items,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

        return items;
    }

    public async Task<int> GetTotalPageAsync()
    {
        return await _context.Clients.CountAsync();
    }

    public async Task<Client> GetClientByIdAsync(int clientId)
    {
        var cached = await CacheHelper.GetAsync<Client>(
            _cache,
            _logger,
            CacheKeys.Client_ById(clientId));
        if (cached != null)
            return cached;

        var entity = await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.Client_ById(clientId),
            entity);

        return entity;
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