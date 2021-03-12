using System;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#else
using System.Windows.Data;
using System.Windows;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    class DateTimeOffsetToLocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CustomCultureInfo language)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is DateTimeOffset universalTime)
            {
                return universalTime.LocalDateTime;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo language)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is DateTime localDateTime)
            {
                return new DateTimeOffset(localDateTime).ToUniversalTime();
            }
            throw new NotImplementedException();
        }
    }
}
