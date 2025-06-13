using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Helpers;

public class StatusToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int statusId && parameter is string targetStatus)
        {
            return targetStatus switch
            {
                "Draft" => statusId == (int)ClientStatusType.Draft ? Visibility.Visible : Visibility.Collapsed,
                "Active" => statusId == (int)ClientStatusType.Active ? Visibility.Visible : Visibility.Collapsed,
                "Rejected" => statusId == (int)ClientStatusType.Rejected ? Visibility.Visible : Visibility.Collapsed,
                "NotDraft" => statusId != (int)ClientStatusType.Draft ? Visibility.Visible : Visibility.Collapsed,
                "NotActive" => statusId != (int)ClientStatusType.Active ? Visibility.Visible : Visibility.Collapsed,
                "NotRejected" => statusId != (int)ClientStatusType.Rejected ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility.Collapsed
            };
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
} 