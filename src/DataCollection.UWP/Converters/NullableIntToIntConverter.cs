/*******************************************************************************
  * Copyright 2020 Esri
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
using Windows.UI.Xaml.Data;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Converters
{
    /// <summary>
    /// Converter applies a format string to a date time
    /// </summary>
    public class NullableIntToIntConverter : IValueConverter
    {
        /// <summary>
        /// Converts an int? to an int
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intvalue)
            {
                return intvalue;
            }
            return -1;
        }

        /// <summary>
        /// Converts an int to an int?
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
