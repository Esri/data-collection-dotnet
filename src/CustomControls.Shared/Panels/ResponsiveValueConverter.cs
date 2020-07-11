using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if __WPF__
using System.Windows.Data;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

#if __UWP__
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Panels
{
    public class ResponsiveValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is bool IsCollapsed)
            {
                if (parameter is string stringParam)
                {
                    var parts = stringParam.Split('|');
                    if (parts.Length > 1)
                    {
                        if (IsCollapsed)
                        {
                            return parts[0];
                        }
                        else
                        {
                            return parts[1];
                        }
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
