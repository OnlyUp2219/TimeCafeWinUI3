using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TimeCafeWinUI3.Helpers
{
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
                return ((ClientStatusType)statusId) switch
                {
                    ClientStatusType.Draft => isBackground ? GridCautionStyle : FontIconCautionStyle,      // Черновик
                    ClientStatusType.Active => isBackground ? GridSuccessStyle : FontIconSuccessStyle,      // Активный
                    ClientStatusType.Rejected => isBackground ? GridCriticalStyle : FontIconCriticalStyle,    // Отказ от услуг
                    _ => isBackground ? GridNeutralStyle : FontIconNeutralStyle
                };
            }

            return isBackground ? GridNeutralStyle : FontIconNeutralStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}