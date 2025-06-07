using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TimeCafeWinUI3.Helpers;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            if (parameter as string == "Inverse")
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility)
        {
            if (parameter as string == "Inverse")
            {
                return visibility != Visibility.Visible;
            }
            return visibility == Visibility.Visible;
        }
        return false;
    }
}