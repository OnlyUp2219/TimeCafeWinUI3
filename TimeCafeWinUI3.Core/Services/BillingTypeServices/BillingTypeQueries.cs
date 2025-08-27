using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.BillingTypeServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.BillingTypeServices
{
    public class BillingTypeQueries : IBillingTypeQueries
    {
        private readonly TimeCafeContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<BillingTypeQueries> _logger;

        public BillingTypeQueries(TimeCafeContext context, IDistributedCache cache, ILogger<BillingTypeQueries> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<BillingType>> GetBillingTypesAsync()
        {

            var cached = await CacheHelper.GetAsync<IEnumerable<BillingType>>(
                _cache,
                _logger,
                CacheKeys.BillingTypes_All);
            if (cached != null)
                return cached;

            var items = await _context.BillingTypes
                .AsNoTracking()
                .OrderBy(b => b.BillingTypeName)
                .ToListAsync();

            await CacheHelper.SetAsync<IEnumerable<BillingType>>(
                _cache,
                _logger,
                CacheKeys.BillingTypes_All,
                items);

            return items;
        }

        public async Task<BillingType> GetBillingTypeByIdAsync(int billingTypeId)
        {
            var cached = await CacheHelper.GetAsync<BillingType>(
                _cache,
                _logger,
                CacheKeys.BillingTypes_ById(billingTypeId)
                );
            if (cached != null)
                return cached;

            var entity = await _context.BillingTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BillingTypeId == billingTypeId);

            await CacheHelper.SetAsync<BillingType>(
                _cache,
                _logger,
                CacheKeys.BillingTypes_ById(billingTypeId),
                entity);

            return entity;
        }
    }
}