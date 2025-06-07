using Microsoft.UI.Xaml.Data;

namespace TimeCafeWinUI3.Helpers
{
    class FullNameInitialsConverters : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Client client)
            {
                return $"{client.LastName} {client.FirstName[0]}. {client.MiddleName[0]}.";
            }
            return string.Empty;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
