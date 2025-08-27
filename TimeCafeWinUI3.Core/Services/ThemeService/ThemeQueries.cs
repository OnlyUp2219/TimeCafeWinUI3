using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.ThemeService;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ThemeService;

public class ThemeQueries : IThemeQueries
{
    private readonly TimeCafeContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ThemeQueries> _logger;
    public ThemeQueries(TimeCafeContext context, IDistributedCache cache, ILogger<ThemeQueries> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Theme>> GetThemesAsync()
    {
        var cached = await CacheHelper.GetAsync<IEnumerable<Theme>>(
            _cache,
            _logger,
            CacheKeys.Themes_All);
        if (cached != null)
            return cached;

        var entity = await _context.Themes
        .AsNoTracking()
        .ToListAsync();

        await CacheHelper.SetAsync(
            _cache,
            _logger,
            CacheKeys.Themes_All,
            entity);

        return entity;
    }
}