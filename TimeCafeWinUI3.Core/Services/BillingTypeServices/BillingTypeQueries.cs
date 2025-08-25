using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services.BillingTypeServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.BillingTypeServices
{
    public class BillingTypeQueries : IBillingTypeQueries
    {
        private readonly TimeCafeContext _context;

        public BillingTypeQueries(TimeCafeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BillingType>> GetBillingTypesAsync()
        {
            return await _context.BillingTypes
                .OrderBy(b => b.BillingTypeName)
                .ToListAsync();
        }

        public async Task<BillingType> GetBillingTypeByIdAsync(int billingTypeId)
        {
            return await _context.BillingTypes
                .FirstOrDefaultAsync(b => b.BillingTypeId == billingTypeId);
        }
    }
}