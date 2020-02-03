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
using System.Globalization;
#if NETFX_CORE
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
    /// Converts null to boolean
    /// </summary>
    class NullToBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            // Handle null to bool and not null (inverse) to bool
            if (parameter != null && parameter.ToString() == "Inverse")
            {
                //if value is null, bool = true (inverse)
                return (value == null) ? true : false;
            }
            else
            {
                //if value is null, bool is false 
                return (value == null) ? false : true;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
