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
using System.Linq;
using System.Collections.Generic;
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
    /// Converts boolean to visibility
    /// </summary>
    class EmptyCollectionToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a collection size to a bool; used to either show a list view or a placeholder message e.g. "No attachments found"
        /// </summary>
        /// <remarks>
        /// If NoSingleton is used as the parameter, collections with a single element are treated as if they were empty.
        /// </remarks>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is IEnumerable<object> collection)
            {
                if (parameter != null && parameter.ToString() == "Inverse")
                {
                    return !collection.Any();
                }
                else if (parameter != null && parameter.ToString() == "NoSingleton")
                {
                    return collection.Count() > 1;
                }
                else
                {
                    return collection.Any();
                }
            }
            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
