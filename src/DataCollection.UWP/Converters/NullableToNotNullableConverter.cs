using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Converters
{
    // Needed because TimePicker can't handle nullable timespan values
    public class NullableToNotNullableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                if(targetType == typeof(TimeSpan))
                {
                    return TimeSpan.Zero;
                }
                else if (targetType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.MinValue;
                }
                else
                {
                    return null;
                }
            }
            if (value.GetType() == typeof(TimeSpan?) && targetType == typeof(TimeSpan))
            {
                TimeSpan? val = value as TimeSpan?;
                if (val.HasValue)
                {
                    return val.Value;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
            else if (value.GetType() == typeof(TimeSpan) && targetType == typeof(TimeSpan?))
            {
                return (TimeSpan?)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }
}
