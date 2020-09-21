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
using System.Windows.Data;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Converters
{
    /// <summary>
    /// Converter to choose MapTitle binding based on which value is available
    /// This is part of the workaround for Runtime bug: OnPropertyChnaged doesn't fire on Map properties
    /// </summary>
    class MapTitleConverter : IMultiValueConverter
    {
        /// <summary>
        /// Chooses whichever value is not null or empty, with proprity given to first value which is MapViewModel.Map.Item.Title
        /// </summary>
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values[0] is string)
            {
                if (string.IsNullOrEmpty(values[0].ToString()))
                    return values[1];
            }
            return values[0];
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
