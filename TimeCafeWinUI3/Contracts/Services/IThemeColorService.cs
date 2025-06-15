using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace TimeCafeWinUI3.Contracts.Services
{
    public interface IThemeColorService
    {
        Style GetThemeBorderStyle(string technicalName);
        LinearGradientBrush GetThemeGradientBrush(string technicalName);
    }
} 