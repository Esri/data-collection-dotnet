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

using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
#if NETFX_CORE
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#else
using System.Windows.Data;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Converters
{
    class DisplayedFieldsToRealDisplayedFieldsConverter : IValueConverter
    {
        // Gets the FormattedValue for PopupFieldValue; uses popup manager for everything but dates.
        // For DateTimes, the value is represented in the local time zone.
        public object Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is IEnumerable<PopupFieldValue> displayedFields)
            {
                return displayedFields.Select(field => new WrappedPopupFieldValue(field));
            }
            else if (value is PopupFieldValue displayedField)
            {
                return new WrappedPopupFieldValue(displayedField);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            return value;
        }
    }
}
