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
using Windows.UI.Xaml.Data;
using Culture = System.String;
#else
using System.Windows.Data;
using Culture = System.Globalization.CultureInfo;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    /// <summary>
    /// Converter for conveniently binding to the inverse of a boolean value
    /// </summary>
    public class BoolInverseConverter : IValueConverter
    {
        /// <summary>
        /// Returns the inverse of the input boolean, or the input value if it is not a boolean.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, Culture language)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }
            return value;
        }

        /// <summary>
        /// Returns the inverse of the input boolean, or the input value if it is not a boolean.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, Culture language)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }
            return value;
        }
    }
}
