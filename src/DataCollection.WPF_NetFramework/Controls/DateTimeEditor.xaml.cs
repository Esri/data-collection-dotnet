/*******************************************************************************
  * Copyright 2021 Esri
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Controls
{
    /// <summary>
    /// Custom control for conveniently editing Date+Time fields
    /// </summary>
    public partial class DateTimeEditor : UserControl
    {
        // Tracks textbox content validity to enable showing a helpful error message
        private bool _isDateInvalid;
        private bool _isTimeInvalid;

        public DateTimeEditor()
        {
            InitializeComponent();
        }

        public DateTime? DateTime
        {
            get { return (DateTime?)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(nameof(DateTime), typeof(DateTime?), typeof(DateTimeEditor), new PropertyMetadata(null, HandleDateTimeChange));

        public static void HandleDateTimeChange(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea)
        {
            DateTimeEditor sendingObject = (DateTimeEditor)dpo;
            sendingObject.UpdateUiForNewDate();
        }

        private void EmbeddedDatePicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SetNewDatePart(EmbeddedDatePicker.SelectedDate.Value);
                UpdateFeedbackUi();
            }
            DatePopup.IsOpen = false;
        }

        private void UpdateUiForNewDate()
        {
            if (!DateTime.HasValue)
            {
                DatePartTextBox.Text = string.Empty;
                TimePartTextBox.Text = string.Empty;
                EmbeddedDatePicker.SelectedDate = null;
                EmbeddedTimePicker.SelectedTime = null;
                _isDateInvalid = false;
                _isTimeInvalid = false;
            }
            else
            {
                DatePartTextBox.Text = DateTime.Value.ToShortDateString();
                TimePartTextBox.Text = DateTime.Value.ToShortTimeString();
                EmbeddedTimePicker.SelectedTime = DateTime.Value.TimeOfDay;
                EmbeddedDatePicker.SelectedDate = DateTime.Value.Date;
            }

            _isDateInvalid = false;
            _isTimeInvalid = false;
            UpdateFeedbackUi();
        }

        private void SetNewDatePart(DateTime datePart)
        {
            // If setting date first, assume current time
            if (DateTime == null)
            {
                DateTime = datePart.Add(System.DateTime.Now.TimeOfDay);
            }
            // else update datetime with new date part
            else
            {
                var oldDateLocaltime = DateTime.Value.TimeOfDay;
                DateTime = datePart.Add(oldDateLocaltime);
            }
        }

        private void SetNewTimePart(TimeSpan? timePart)
        {
            // If setting time first, assume current date
            if (DateTime == null && timePart != null)
            {
                DateTime = System.DateTime.Now.Date.Add(timePart.Value);
                UpdateUiForNewDate();
            }
            // else update datetime with new time part
            else if (DateTime != null && timePart != null)
            {
                DateTime = DateTime.Value.Date.Add(timePart.Value);
                UpdateUiForNewDate();
            }
        }

        private void DatePartTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            string dateInProgress = DatePartTextBox.Text;

            if (string.IsNullOrWhiteSpace(dateInProgress))
            {
                DateTime = null;
            }
            else
            {
                if (System.DateTime.TryParse(dateInProgress, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var newResult))
                {
                    SetNewDatePart(newResult);
                }
                else
                {
                    _isDateInvalid = true;
                    UpdateFeedbackUi();
                }
            }
        }

        private void TimePartTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            var timeInProgress = TimePartTextBox.Text;

            if (string.IsNullOrWhiteSpace(timeInProgress))
            {
                SetNewTimePart(TimeSpan.Zero);
            }
            else
            {
                if (System.DateTime.TryParse(timeInProgress, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var newResult))
                {
                    SetNewTimePart(newResult.TimeOfDay);
                }
                else
                {
                    _isTimeInvalid = true;
                    UpdateFeedbackUi();
                }
            }
        }

        private void UpdateFeedbackUi()
        {
            if (DateTime.HasValue)
            {
                PreviewTextBox.Text = string.Format(Shared.Properties.Resources.GetString("DTE_Preview_FormatString"),
                    DateTime.Value.ToShortDateString(), DateTime.Value.ToShortTimeString());
            }
            else
            {
                PreviewTextBox.Text = Shared.Properties.Resources.GetString("DTE_EmptyDate_Message");
            }
            ErrorBox.Visibility = (_isDateInvalid || _isTimeInvalid) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime == null)
            {
                UpdateUiForNewDate();
            }
            else
            {
                DateTime = null;
            }
        }

        // Automatically select textbox contents on focus (improves editing experience)
        private void TextBox_GotFocus(object sender, RoutedEventArgs e) => ((TextBox)sender).SelectAll();

        private void Date_Button_Click(object sender, RoutedEventArgs e) => DatePopup.IsOpen = true;

        private void Time_Button_Click(object sender, RoutedEventArgs e) => TimePopup.IsOpen = true;

        private void EmbeddedTimePicker_SelectedTimeChanged(object sender, EventArgs e)
        {
            if (EmbeddedTimePicker.SelectedTime.HasValue)
            {
                SetNewTimePart(EmbeddedTimePicker.SelectedTime.Value);
            }
            UpdateFeedbackUi();
            TimePopup.IsOpen = false;
        }

        private void EmbeddedTimePicker_DismissRequested(object sender, EventArgs e) => TimePopup.IsOpen = false;
    }
}
