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

using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Controls
{
    /// <summary>
    /// Custom calendar date picker works around issue with null date binding in native control.
    /// Using Binding, CalendarDatePicker shows null as 1/1/1919. With x:Bind, the control calls Set on
    /// whatever is bound to the Date property when that value raises a PCE, causing an exception when
    /// finishing the popup manager editing session.
    /// See this StackOverflow post for details: https://stackoverflow.com/questions/39775527/calendardatepicker-binding-null-value
    /// </summary>
    public class NullReadyCalendarDatePicker : CalendarDatePicker
    {
        public NullReadyCalendarDatePicker()
        {
            DateChanged += NullReadyCalendarDatePicker_DateChanged;
        }

        private void NullReadyCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            // If the new date looks like it should be null, set it to null.
            if (args.NewDate.HasValue && args.NewDate.Value == MinDate)
            {
                Date = null;
            }
        }
    }
}
