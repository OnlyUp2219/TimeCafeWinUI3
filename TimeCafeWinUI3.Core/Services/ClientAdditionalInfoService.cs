using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class ClientAdditionalInfoService : IClientAdditionalInfoService
{
    private readonly TimeCafeContext _context;

    public ClientAdditionalInfoService(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClientAdditionalInfo>> GetClientAdditionalInfosAsync(int clientId)
    {
        return await _context.ClientAdditionalInfos
            .Where(i => i.ClientId == clientId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<ClientAdditionalInfo> CreateAdditionalInfoAsync(ClientAdditionalInfo info)
    {
        info.CreatedAt = DateTime.Now;
        _context.ClientAdditionalInfos.Add(info);
        await _context.SaveChangesAsync();
        return info;
    }

    public async Task<ClientAdditionalInfo> UpdateAdditionalInfoAsync(ClientAdditionalInfo info)
    {
        var existingInfo = await _context.ClientAdditionalInfos.FindAsync(info.InfoId);
        if (existingInfo == null)
            throw new KeyNotFoundException($"Дополнительная информация с ID {info.InfoId} не найдена");

        _context.Entry(existingInfo).CurrentValues.SetValues(info);
        await _context.SaveChangesAsync();
        return info;
    }

    public async Task<bool> DeleteAdditionalInfoAsync(int infoId)
    {
        var info = await _context.ClientAdditionalInfos.FindAsync(infoId);
        if (info == null)
            return false;

        _context.ClientAdditionalInfos.Remove(info);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ClientAdditionalInfo> GetAdditionalInfoByIdAsync(int infoId)
    {
        return await _context.ClientAdditionalInfos
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.InfoId == infoId);
    }
} 