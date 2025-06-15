using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class BillingTypeService : IBillingTypeService
{
    private readonly TimeCafeContext _context;

    public BillingTypeService(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BillingType>> GetBillingTypesAsync()
    {
        return await _context.BillingTypes
            .OrderBy(b => b.BillingTypeName)
            .ToListAsync();
    }
} 