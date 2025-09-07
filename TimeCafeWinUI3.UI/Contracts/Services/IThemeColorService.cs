namespace TimeCafeWinUI3.UI.Contracts.Services;

public interface IThemeColorService
{
    Style GetThemeBorderStyle(string technicalName);
    LinearGradientBrush GetThemeGradientBrush(string technicalName);
}