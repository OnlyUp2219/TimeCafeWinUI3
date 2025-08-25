using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services.ClientAdditionalServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientAdditionalServices;

public class ClientAdditionalInfoQueries : IClientAdditionalInfoQueries
{
    private readonly TimeCafeContext _context;

    public ClientAdditionalInfoQueries(TimeCafeContext context)
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

    public async Task<ClientAdditionalInfo> GetAdditionalInfoByIdAsync(int infoId)
    {
        return await _context.ClientAdditionalInfos
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.InfoId == infoId);
    }
}
