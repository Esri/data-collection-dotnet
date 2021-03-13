using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Controls
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
            DataContext = this;
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

        // Using a DependencyProperty as the backing store for SelectedTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(TimeSpan?), typeof(TimePicker), new PropertyMetadata(null, HandleTimeChanged));




        public static void HandleTimeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea)
        {
            TimePicker sendingObject = (TimePicker)dpo;

            sendingObject.HandleTimeChange((TimeSpan?)dpcea.NewValue);
        }

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

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            HandleTimeChange(SelectedTime);
            DismissRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler SelectedTimeChanged;

        public event EventHandler DismissRequested;
    }
}
