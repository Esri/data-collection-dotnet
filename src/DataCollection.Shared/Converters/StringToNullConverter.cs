using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if __UWP__
using lang = System.String;
using Windows.UI.Xaml.Data;
#elif WPF
using lang = System.Globalization.CultureInfo;
using System.Windows.Data;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    /// <summary>
    /// Special converter used for two-way bindings where null is acceptable but an empty string isn't
    /// </summary>
    public class StringToNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, lang culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, lang culture)
        {
            if (value is string stringvalue)
            {
                if (string.IsNullOrEmpty(stringvalue))
                {
                    return null;
                }
            }
            return value;
        }
    }
}
