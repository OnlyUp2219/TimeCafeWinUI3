using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.ClientAdditionalServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientAdditionalServices;

public class ClientAdditionalInfoQueries : IClientAdditionalInfoQueries
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ClientAdditionalInfoQueries> _logger;

    public ClientAdditionalInfoQueries(TimeCafeContext context, IDistributedCache cache, ILogger<ClientAdditionalInfoQueries> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<ClientAdditionalInfo>> GetClientAdditionalInfosAsync(int clientId)
    {
        var cached = await CacheHelper.GetAsync<IEnumerable<ClientAdditionalInfo>>(
            _cache,
            _logger,
            CacheKeys.ClientAdditionalInfo_All);
        if (cached != null)
            return cached;

        var entity = await _context.ClientAdditionalInfos
            .Where(i => i.ClientId == clientId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.ClientAdditionalInfo_All,
            entity);

        return entity;
    }

    public async Task<ClientAdditionalInfo> GetAdditionalInfoByIdAsync(int infoId)
    {
        var cached = await CacheHelper.GetAsync<ClientAdditionalInfo>(
            _cache,
            _logger,
            CacheKeys.ClientAdditionalInfo_ById(infoId));
        if (cached != null)
            return cached;

        var entity = await _context.ClientAdditionalInfos
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.InfoId == infoId);

        await CacheHelper.SetAsync(
             _cache,
             _logger,
             CacheKeys.ClientAdditionalInfo_ById(infoId),
             entity);

        return entity;
    }
}
