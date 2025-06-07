//using Microsoft.UI.Xaml.Data;

//namespace TimeCafeWinUI3.Helpers
//{
//    public class StatusTextConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            if (value is Client client)
//            {
//                return ((ClientStatusType)client.StatusId) switch
//                {
//                    ClientStatusType.Draft => "Черновик",
//                    ClientStatusType.Active => "Активный",
//                    ClientStatusType.Rejected => "Отказ от услуг",
//                    _ => "Неизвестный статус"
//                };
//            }
//            return "Неизвестный статус";
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}