using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace TimeCafeWinUI3.UI.Utilities.Converters;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        Style GridSuccessStyle = (Style)Application.Current.Resources["GridDynamicColorSuccess"];
        Style GridCautionStyle = (Style)Application.Current.Resources["GridDynamicColorCaution"];
        Style GridCriticalStyle = (Style)Application.Current.Resources["GridDynamicColorCritical"];
        Style GridNeutralStyle = (Style)Application.Current.Resources["GridDynamicColorNeutral"];

        Style FontIconSuccessStyle = (Style)Application.Current.Resources["FontIconDynamicColorSuccess"];
        Style FontIconCautionStyle = (Style)Application.Current.Resources["FontIconDynamicColorCaution"];
        Style FontIconCriticalStyle = (Style)Application.Current.Resources["FontIconDynamicColorCritical"];
        Style FontIconNeutralStyle = (Style)Application.Current.Resources["FontIconDynamicColorNeutral"];

        bool isBackground = parameter?.ToString()?.ToLower() == "background";
        // TODO : Убрать statusId и вернутьк Status.StatusName
        if (value is int statusId)
        {
            return (EClientStatusType)statusId switch
            {
                EClientStatusType.Draft => isBackground ? GridCautionStyle : FontIconCautionStyle,      // Черновик
                EClientStatusType.Active => isBackground ? GridSuccessStyle : FontIconSuccessStyle,      // Активный
                EClientStatusType.Rejected => isBackground ? GridCriticalStyle : FontIconCriticalStyle,    // Отказ от услуг
                _ => isBackground ? GridNeutralStyle : FontIconNeutralStyle
            };
        }

        // Для статуса возвращаем цвет текста
        if (parameter == "status" && value is bool isActive)
        {
            return isActive
                ? new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)) // Зеленый для активного
                : new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Красный для неактивного
        }

        // Для транзакций возвращаем цвет текста
        if (parameter == "transaction" && value is int transactionTypeId)
        {
            return transactionTypeId == 1
                ? new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)) // Зеленый для пополнения
                : new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Красный для списания
        }

        return isBackground ? GridNeutralStyle : FontIconNeutralStyle;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}