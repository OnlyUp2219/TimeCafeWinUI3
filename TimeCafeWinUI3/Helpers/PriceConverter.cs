using Microsoft.UI.Xaml.Data;
using System;

namespace TimeCafeWinUI3.Helpers;

public class PriceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal price)
        {
            return $"{Math.Round(price, 2):N2} ₽";
        }
        return "0 ₽";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
} 