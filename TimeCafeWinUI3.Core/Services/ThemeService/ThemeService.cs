using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services.ThemeService;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ThemeService;

public class ThemeQueries : IThemeQueries
{
    private readonly TimeCafeContext _context;

    public ThemeQueries(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Theme>> GetThemesAsync()
    {
        return await _context.Themes
            .ToListAsync();
    }
}