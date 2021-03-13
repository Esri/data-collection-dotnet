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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Controls
{
    /// <summary>
    /// Control for selecting a time using mouse or touch
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
            // Ensure control is set up with a default value
            HandleTimeChange(null);
        }

        public List<int> ValidHours { get; } = Enumerable.Range(1, 12).ToList();
        public List<int> ValidMinutes { get; } = Enumerable.Range(0, 60).ToList();
        public List<string> AMPMValues { get; } = new List<string> { "AM", "PM" };

        public TimeSpan? SelectedTime
        {
            get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(nameof(SelectedTime), typeof(TimeSpan?), typeof(TimePicker), new PropertyMetadata(null, HandleTimeChanged));

        public static void HandleTimeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea) => ((TimePicker)dpo).HandleTimeChange((TimeSpan?)dpcea.NewValue);

        private void HandleTimeChange(TimeSpan? newTime)
        {
            if (newTime.HasValue)
            {
                HourPicker.SelectedItem = newTime.Value.Hours % 12;
                MinutePicker.SelectedItem = newTime.Value.Minutes;
                AMPMPicker.SelectedIndex = newTime.Value.Hours > 11 ? 1 : 0;
            }
            else
            {
                HourPicker.SelectedIndex = 11;
                MinutePicker.SelectedIndex = 0;
                AMPMPicker.SelectedIndex = 0;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SelectedTime = DateTime.Parse($"{DateTime.Now.ToShortDateString()} {HourPicker.SelectedItem}:{MinutePicker.SelectedItem} {AMPMPicker.SelectedItem}").TimeOfDay;
            SelectedTimeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reset value and notify listeners that close is requested
        /// </summary>
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            HandleTimeChange(SelectedTime);
            DismissRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised when the user commits/saves the selection
        /// </summary>
        public event EventHandler SelectedTimeChanged;

        /// <summary>
        /// Raised when the user cancels or clears the changes
        /// </summary>
        public event EventHandler DismissRequested;
    }
}
