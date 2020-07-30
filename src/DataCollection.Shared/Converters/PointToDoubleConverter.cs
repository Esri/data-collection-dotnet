using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#if __UWP__
using Windows.UI.Xaml.Data;
using Culture = System.String;
using Windows.Foundation;
#else
using System.Windows.Data;
using Culture = System.Globalization.CultureInfo;
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    public class PointToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, Culture language)
        {
            if (parameter is string positionString && value is Point screenpoint)
            {
                string position = null;
                double offset = 0;
                var parts = positionString.Split('|');

                if (parts.Length > 0)
                {
                    position = parts[0];
                }
                if (parts.Length > 1)
                {
                    Double.TryParse(parts[1], out offset);
                }
                switch (position)
                {
                    case "Top":
                    case "Y":
                        return screenpoint.Y + offset;
                    case "Left":
                    case "X":
                        return screenpoint.X + offset;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, Culture language)
        {
            throw new NotImplementedException();
        }
    }
}
