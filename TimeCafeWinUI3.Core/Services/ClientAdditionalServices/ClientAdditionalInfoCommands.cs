using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.ClientAdditionalServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientAdditionalServices;

public class ClientAdditionalInfoCommands : IClientAdditionalInfoCommands
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ClientAdditionalInfoCommands> _logger;

    public ClientAdditionalInfoCommands(TimeCafeContext context, IDistributedCache cache, ILogger<ClientAdditionalInfoCommands> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ClientAdditionalInfo> CreateAdditionalInfoAsync(ClientAdditionalInfo info)
    {
        info.CreatedAt = DateTime.Now;
        _context.ClientAdditionalInfos.Add(info);
        await _context.SaveChangesAsync();

        await CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.ClientAdditionalInfo_All);

        return info;
    }

    public async Task<ClientAdditionalInfo> UpdateAdditionalInfoAsync(ClientAdditionalInfo info)
    {
        var existingInfo = await _context.ClientAdditionalInfos.FindAsync(info.InfoId);
        if (existingInfo == null)
        {
            _logger.LogWarning("Попытка обновить ClientAdditionalInfo с несуществующим Id={Id}", info.InfoId);
            throw new KeyNotFoundException($"Дополнительная информация с ID {info.InfoId} не найдена");
        }

        _context.Entry(existingInfo).CurrentValues.SetValues(info);
        await _context.SaveChangesAsync();

        await CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.ClientAdditionalInfo_All,
            CacheKeys.ClientAdditionalInfo_ById(info.InfoId)
        );

        return info;
    }


    public async Task<bool> DeleteAdditionalInfoAsync(int infoId)
    {
        var info = await _context.ClientAdditionalInfos.FindAsync(infoId);
        if (info == null)
            return false;

        _context.ClientAdditionalInfos.Remove(info);
        await _context.SaveChangesAsync();

        await CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.ClientAdditionalInfo_All,
            CacheKeys.ClientAdditionalInfo_ById(info.InfoId)
        );

        return true;
    }
}