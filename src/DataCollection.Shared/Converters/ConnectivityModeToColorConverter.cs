/*******************************************************************************
  * Copyright 2019 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using System;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using System.Windows.Media;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#else
using System.Windows.Data;
using System.Windows;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Converters
{
    /// <summary>
    /// Converts ConnectivityMode to color to be used for the app banner
    /// </summary>
    class ConnectivityModeToColorConverter : IValueConverter
    {
        /// <summary>
        /// Handle the conversion from a boolean value to a color value
        /// </summary>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is ConnectivityMode)
            {
                //if value is ConnectivityMode.Offline, color = gray
                return ((ConnectivityMode)value == ConnectivityMode.Offline) ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Green);
            }
            else
                return null;
        }

        /// <summary>
        /// Handle the conversion from a color value to a ConnectivityMode value
        /// </summary>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is Color)
            {
                    //if color is gray return ConnectivityMode.Offline
                    return ((Color)value == Colors.Gray)? ConnectivityMode.Offline : ConnectivityMode.Online;
            }
            else
                return null;
        }
    }
}

