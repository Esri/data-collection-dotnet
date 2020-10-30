/*******************************************************************************
  * Copyright 2019 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  https://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using System;
using System.Globalization;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#else
using System.Windows.Data;
using System.Windows;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif


namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{

    /// <summary>
    /// Converts int to visibility
    /// </summary>
    class ProgressToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Handle the conversion from an int representing progress value to a visibility value
        /// </summary>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is int)
            {
                //if value is 0 or 100, visibility = collapsed
                return ((int)value <= 0 || (int)value >= 100) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                return Visibility.Collapsed;
        }

        /// <summary>
        /// Converting back isn't supported as a non-zero value cannot be generated
        /// </summary>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
