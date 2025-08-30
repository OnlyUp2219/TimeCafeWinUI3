using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Repositories;

public interface IThemeRepository
{
    Task<IEnumerable<Theme>> GetThemesAsync();
}