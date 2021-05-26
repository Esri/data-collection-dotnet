using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class CollectionIsEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, Culture culture)
        {
            if (value is int collectionSize)
            {
                if (parameter is string stringparameter && stringparameter == "Empty")
                {
                    return collectionSize == 0;
                }
                return collectionSize != 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, Culture culture)
        {
            throw new NotImplementedException();
        }
    }
}
