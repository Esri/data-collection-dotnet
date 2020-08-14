using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Converters
{
    public class BoolToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool flag)
            {
                if (flag)
                {
                    // Arrow with hourglass or spinner
                    return Cursors.AppStarting;
                }
            }

            return Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
