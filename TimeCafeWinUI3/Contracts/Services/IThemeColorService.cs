using Microsoft.UI.Xaml;

namespace TimeCafeWinUI3.Contracts.Services
{
    public interface IThemeColorService
    {
        Style GetThemeBorderStyle(string technicalName);
    }
} 