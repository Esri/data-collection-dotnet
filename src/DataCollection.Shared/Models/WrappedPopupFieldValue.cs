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
using System.ComponentModel;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models
{
    /// <summary>
    /// Class wraps PopupFieldValue to enable proper data binding support with localized formatted date values.
    /// Needed because FormattedValue formats dates in UTC, which causes awkward behavior when editing, since users expect local time zone.
    /// A converter would work, but then you'd need to bind to the PopupField itself, which breaks property change notification for the FormattedValue property.
    /// </summary>
    public class WrappedPopupFieldValue : INotifyPropertyChanged
    {
        private PopupFieldValue _wrappedValue;

        public WrappedPopupFieldValue(PopupFieldValue baseValue)
        {
            _wrappedValue = baseValue;
            _wrappedValue.PropertyChanged += wrappedValue_propertyChanged;
        }

        private void wrappedValue_propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);

            // Re-raise any property change events
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));

            if (e.PropertyName == nameof(_wrappedValue.FormattedValue))
            {
                // Also raise property change events for Formatted Value, because it may have changed.
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReformattedValue)));
            }
        }

        /// <summary>
        /// Converts dates before formatting the way <see cref="FormattedValue"/> would.
        /// If you want times in UTC, bind to <see cref="FormattedValue"/> directly.
        /// </summary>
        public string ReformattedValue
        {
            get
            {
                if (Value is DateTimeOffset dateValue)
                {
                    var localDateTime = dateValue.ToLocalTime().DateTime;

                    switch (Field.Format?.DateFormat)
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
                        case null:
                        default:
                            return localDateTime.ToString();
                    }
                }

                return FormattedValue;
            }
        }

        // Wrap PopupFieldValue properties for binding convenience.
        public PopupField Field => _wrappedValue.Field;
        public object Value { get { try { return _wrappedValue.Value; } catch (Exception) { return null; } } }
        public string FormattedValue { get { try { return _wrappedValue.FormattedValue; } catch (Exception) { return null; } } }
        public Exception ValidationError => _wrappedValue.ValidationError;
        public object OriginalValue => _wrappedValue.OriginalValue;
        public bool HasChanges => _wrappedValue.HasChanges;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
