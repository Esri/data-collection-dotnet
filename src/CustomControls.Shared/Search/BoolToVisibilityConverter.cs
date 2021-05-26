using System;
using System.Collections.Generic;
using System.Text;
#if __WPF__
using System.Windows;
using System.Windows.Data;
using Culture = System.Globalization.CultureInfo;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Culture = System.String;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, Culture culture)
        {
            if (value is bool inputValue)
            {
                if ("Inverse".Equals(parameter))
                {
                    return inputValue ? Visibility.Collapsed : Visibility.Visible;
                }
                return inputValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, Culture culture)
        {
            throw new NotImplementedException();
        }
    }
}
