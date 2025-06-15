using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;
 
public interface IThemeService
{
    Task<IEnumerable<Theme>> GetThemesAsync();
} 