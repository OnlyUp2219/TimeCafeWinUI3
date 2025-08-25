using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.ThemeService;
 
public interface IThemeQueries
{
    Task<IEnumerable<Theme>> GetThemesAsync();
} 