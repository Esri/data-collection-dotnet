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
#if NETFX_CORE
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#else
using System.Windows.Data;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    /// <summary>
    /// Converter converts to local time zone and then applies the given format string.
    /// </summary>
    public class DateTimeFormatConverter : IValueConverter
    {
        /// <summary>
        /// Applies format string specified in <paramref name="parameter"/> to the input <paramref name="value"/>, which is expected to
        /// be a <see cref="DateTime"/> or <see cref="DateTimeOffset"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CustomCultureInfo language)
        {
            if (parameter is string format)
            {
                if (value is DateTimeOffset dto)
                {
                    return dto.ToLocalTime().ToString(format);
                }
                else if (value is DateTime date)
                {
                    return date.ToLocalTime().ToString(format);
                }
            }

            return value?.ToString();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo language) => throw new NotImplementedException();
    }
}
