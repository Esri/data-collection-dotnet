using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Converters
{
    class DateTimeOffsetToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return (DateTime?)null;
            }
            else if (value is DateTimeOffset universalTime)
            {
                return universalTime.LocalDateTime;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
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
