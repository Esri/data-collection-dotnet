/*******************************************************************************
  * Copyright 2020 Esri
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
#if __UWP__
using lang = System.String;
using Windows.UI.Xaml.Data;
#else
using lang = System.Globalization.CultureInfo;
using System.Windows.Data;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    /// <summary>
    /// Special converter used for two-way bindings where null is acceptable but an empty string isn't
    /// </summary>
    /// <remarks>This is needed when binding to double fields in popups. Without this converter, deleting the contents of a textbox, or editing an already empty textbox, will result in a validation error from popup manager.</remarks>
    public class StringToNullConverter : IValueConverter
    {
        /// <summary>
        /// Returns the input value unmodified.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, lang culture) => value;

        /// <summary>
        /// For input strings, returns null if the string is null or empty. Returns unmodified value for all other inputs.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, lang culture)
        {
            if (value is string stringValue)
            {
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }
            }
            return value;
        }
    }
}
