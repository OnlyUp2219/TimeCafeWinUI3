//using Microsoft.UI.Xaml;
//using Microsoft.UI.Xaml.Data;
//using System;
//using Windows.Media.Core;

//namespace TimeCafeWinUI3.Helpers
//{
//    public class StringToMediaSourceConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            if (value is string mediaUri && !string.IsNullOrEmpty(mediaUri))
//            {
//                Uri uri;
//                if (Uri.TryCreate(mediaUri, UriKind.Absolute, out uri))
//                {
//                    return MediaSource.CreateFromUri(uri); 
//                }
//                else
//                {
//                    try
//                    {
//                        uri = new Uri("ms-appx://" + mediaUri); 
//                        return MediaSource.CreateFromUri(uri);  
//                    }
//                    catch (UriFormatException)
//                    {
//                        return null;
//                    }
//                }
//            }
//            return null;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}