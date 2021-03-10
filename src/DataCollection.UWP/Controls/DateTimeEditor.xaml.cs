using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Controls
{
    public sealed partial class DateTimeEditor : UserControl
    {
        private bool _isDateInvalid;
        private bool _isTimeInvalid;

        public DateTimeEditor()
        {
            this.InitializeComponent();
        }

        public DateTime? DateTime
        {
            get { return (DateTime?)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime?), typeof(DateTimeEditor), new PropertyMetadata(null, HandleDateTimeChange));

        public static void HandleDateTimeChange(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea)
        {
            DateTimeEditor sendingObject = (DateTimeEditor)dpo;
            sendingObject.DatePartTextBox.LostFocus -= sendingObject.DatePartTextBox_TextChanged;
            sendingObject.TimePartTextBox.LostFocus -= sendingObject.TimePartTextBox_TextChanged;
            if (dpcea.NewValue == null)
            {
                sendingObject.DatePartTextBox.Text = string.Empty;
                sendingObject.TimePartTextBox.Text = string.Empty;
            }
            else if (dpcea.NewValue is DateTime newDateTime)
            {
                sendingObject.DatePartTextBox.Text = newDateTime.ToShortDateString();
                sendingObject.TimePartTextBox.Text = newDateTime.ToShortTimeString();
            }
            sendingObject.DatePartTextBox.LostFocus += sendingObject.DatePartTextBox_TextChanged;
            sendingObject.TimePartTextBox.LostFocus += sendingObject.TimePartTextBox_TextChanged;
        }

        private void SetNewDatePart(DateTime datePart)
        {
            // If setting date first, assume current time
            if (DateTime == null)
            {
                this.DateTime = datePart.Add(System.DateTime.Now.TimeOfDay);
            }
            // else update datetime with new date part
            else
            {
                var oldDateLocaltime = this.DateTime.Value.TimeOfDay;
                this.DateTime = datePart.Add(oldDateLocaltime);
            }
            _isDateInvalid = false;
        }

        private void SetNewTimePart(TimeSpan timePart)
        {
            // If setting time first, assume current date
            if (DateTime == null)
            {
                this.DateTime = System.DateTime.Now.Add(timePart);
            }
            // else update datetime with new time part
            else
            {
                this.DateTime = this.DateTime.Value.Date.Add(timePart);
            }
            _isTimeInvalid = false;
        }

        private void DatePartTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            string dateInProgress = DatePartTextBox.Text;

            if (string.IsNullOrWhiteSpace(dateInProgress))
            {
                this.DateTime = null;
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
                }
            }
            UpdateErrorVisibility();
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
                }
            }
            UpdateErrorVisibility();
        }

        private void UpdateErrorVisibility()
        {
            if (DateTime.HasValue)
            {
                var localTime = DateTime.Value;
                PreviewTextBox.Text = $"Saved as: {localTime.ToShortDateString()} {localTime.ToShortTimeString()} (local time)";
            }
            else
            {
                PreviewTextBox.Text = "Field will be cleared upon save";
            }
            ErrorBox.Visibility = (_isDateInvalid || _isTimeInvalid) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime = null;
            UpdateErrorVisibility();
        }
    }
}
