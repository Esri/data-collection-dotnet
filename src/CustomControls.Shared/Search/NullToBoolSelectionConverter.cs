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
    public class NullToBoolSelectionConverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, Culture culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, Culture culture)
        {
            return null;
        }
    }
}
