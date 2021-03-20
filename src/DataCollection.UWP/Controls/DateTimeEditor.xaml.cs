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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Controls
{
    public sealed partial class DateTimeEditor : UserControl
    {
        private bool _isDateInvalid;
        private bool _isTimeInvalid;

        public DateTimeEditor()
        {
            InitializeComponent();
            UpdateFeedbackUi();
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
            sendingObject.UnsubscribeFromEvents();
            sendingObject.UpdateUiForNewDate((DateTime?)dpcea.NewValue);
            sendingObject.ResubscribeEvents();
        }

        private void UnsubscribeFromEvents()
        {
            DatePartTextBox.LostFocus -= DatePartTextBox_TextChanged;
            TimePartTextBox.LostFocus -= TimePartTextBox_TextChanged;
            EmbeddedDatePicker.DateChanged -= EmbeddedDatePicker_DateChanged;
            EmbeddedTimePicker.TimeChanged -= EmbeddedTimePicker_TimeChanged;
        }

        private void ResubscribeEvents()
        {
            DatePartTextBox.LostFocus += DatePartTextBox_TextChanged;
            TimePartTextBox.LostFocus += TimePartTextBox_TextChanged;
            EmbeddedDatePicker.DateChanged += EmbeddedDatePicker_DateChanged;
            EmbeddedTimePicker.TimeChanged += EmbeddedTimePicker_TimeChanged;
        }

        private void UpdateUiForNewDate(DateTime? newValue)
        {
            if (!newValue.HasValue)
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
                DatePartTextBox.Text = newValue.Value.ToShortDateString();
                TimePartTextBox.Text = newValue.Value.ToShortTimeString();
                EmbeddedTimePicker.SelectedTime = newValue.Value.TimeOfDay;
                EmbeddedDatePicker.SelectedDate = newValue.Value.Date;
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

        private void EmbeddedDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            SetNewDatePart(e.NewDate.LocalDateTime.Date);
            UpdateFeedbackUi();
            this.DatePickerFlyout.Hide();
        }

        private void EmbeddedTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            SetNewTimePart(e.NewTime);
            UpdateFeedbackUi();
            this.TimePickerFlyout.Hide();
        }

        private void SetNewTimePart(TimeSpan timePart)
        {
            // If setting time first, assume current date
            if(DateTime == null && timePart != TimeSpan.Zero)
            { 
                DateTime = System.DateTime.Now.Date.Add(timePart);
            }
            // else update datetime with new time part
            else if (DateTime != null)
            {
                DateTime = DateTime.Value.Date.Add(timePart);
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

        private void Reset_Button_Click(object sender, RoutedEventArgs e) => DateTime = null;

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Automatically select textbox contents on focus (improves editing experience)
            ((TextBox)sender).SelectAll();
        }
    }
}
