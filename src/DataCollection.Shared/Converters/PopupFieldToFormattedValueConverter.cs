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
using System.Globalization;
#if NETFX_CORE
using Windows.UI.Xaml.Data;
using CustomCultureInfo = System.String;
#else
using System.Windows.Data;
using System.Windows;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Converters
{
    class PopupFieldToFormattedValueConverter : IValueConverter
    {
        // Gets the FormattedValue for PopupFieldValue; uses popup manager for everything but dates.
        // For DateTimes, the value is represented in the local time zone.
        public object Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (parameter is PopupFieldValue fieldValue)
            {
                if (fieldValue.Value is DateTimeOffset dateTimeValue)
                {
                    var localDateTime = dateTimeValue.ToLocalTime().DateTime;

                    if (fieldValue.Field.Format?.DateFormat != null)
                    {
                        switch (fieldValue.Field.Format.DateFormat)
                        {
                            case PopupDateFormat.DayShortMonthYear:
                                return localDateTime.ToString("d MMM yyyy");
                            case PopupDateFormat.LongDate:
                                return localDateTime.ToString("dddd, MMMM d, yyyy");
                            case PopupDateFormat.LongMonthDayYear:
                                return localDateTime.ToString("MMMM d, yyyy");
                            case PopupDateFormat.LongMonthYear:
                                return localDateTime.ToString("MMMM yyyy");
                            case PopupDateFormat.ShortDate:
                                return localDateTime.ToString("M/d/yyyy");
                            case PopupDateFormat.ShortDateLE:
                                return localDateTime.ToString("d/M/yyyy");
                            case PopupDateFormat.ShortDateLELongTime:
                                return localDateTime.ToString("d/M/yyyy h:mm:ss tt");
                            case PopupDateFormat.ShortDateLELongTime24:
                                return localDateTime.ToString("d/M/yyyy H:mm:ss");
                            case PopupDateFormat.ShortDateLEShortTime:
                                return localDateTime.ToString("d/M/yyyy h:mm tt");
                            case PopupDateFormat.ShortDateLEShortTime24:
                                return localDateTime.ToString("d/M/yyyy H:mm");
                            case PopupDateFormat.ShortDateLongTime:
                                return localDateTime.ToString("M/d/yyyy h:mm:ss tt");
                            case PopupDateFormat.ShortDateLongTime24:
                                return localDateTime.ToString("M/d/yyyy H:mm:ss");
                            case PopupDateFormat.ShortDateShortTime:
                                return localDateTime.ToString("M/d/yyyy h:mm tt");
                            case PopupDateFormat.ShortDateShortTime24:
                                return localDateTime.ToString("M/d/yyyy H:mm");
                            case PopupDateFormat.ShortMonthYear:
                                return localDateTime.ToString("MMM yyyy");
                            case PopupDateFormat.Year:
                                return localDateTime.ToString();
                        }
                    }
                    return localDateTime.ToString();
                }
                else
                {
                    return fieldValue.FormattedValue;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            return value;
        }
    }
}
