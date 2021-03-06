using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Converters
{
    public class DateTimeOffsetToJustTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dto)
            {
                return dto.LocalDateTime.TimeOfDay;
            }
            else if (value is DateTime dt)
            {
                return dt.ToLocalTime().TimeOfDay;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
