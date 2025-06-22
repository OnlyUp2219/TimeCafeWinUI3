using Microsoft.UI.Xaml.Data;

namespace TimeCafeWinUI3.Helpers;

public class ThemeToBrushConverter : IValueConverter
{
    private static readonly IThemeColorService _themeColorService = App.GetService<IThemeColorService>();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string technicalName)
        {
            return _themeColorService.GetThemeGradientBrush(technicalName);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}