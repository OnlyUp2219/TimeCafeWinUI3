using Microsoft.EntityFrameworkCore;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class ThemeService : IThemeService
{
    private readonly TimeCafeContext _context;

    public ThemeService(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Theme>> GetThemesAsync()
    {
        return await _context.Themes
            .OrderBy(t => t.ThemeName)
            .ToListAsync();
    }
} 