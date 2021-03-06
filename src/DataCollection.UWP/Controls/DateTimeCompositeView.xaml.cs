using System;
using System.Collections.Generic;
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
namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Controls
{
    public sealed partial class DateTimeCompositeView : UserControl
    {
        public DateTimeCompositeView()
        {
            this.InitializeComponent();
        }



        public DateTimeOffset? DateTime
        {
            get { return (DateTimeOffset?)GetValue(DateTimeProperty); }
            set {
                try
                {
                    SetValue(DateTimeProperty, value);
                }
                catch (NullReferenceException)
                {
                }
            }
        }



        public DateTimeOffset? DatePart
        {
            get { return (DateTimeOffset?)GetValue(DatePartProperty); }
            set { SetValue(DatePartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatePart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatePartProperty =
            DependencyProperty.Register("DatePart", typeof(DateTimeOffset?), typeof(DateTimeCompositeView), new PropertyMetadata(null, DatePartPropertyChange));



        public TimeSpan? TimePart
        {
            get { return (TimeSpan?)GetValue(TimePartProperty); }
            set { SetValue(TimePartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimePart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimePartProperty =
            DependencyProperty.Register("TimePart", typeof(TimeSpan?), typeof(DateTimeCompositeView), new PropertyMetadata(null, TimePartPropertyChange));



        // Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(nameof(DateTime), typeof(DateTimeOffset?), typeof(DateTimeCompositeView), new PropertyMetadata(null, DatePropertyChange));

        private static void DatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimeCompositeView view && e.NewValue is DateTimeOffset dto)
            {
                view.DatePart = dto.ToLocalTime().Date;
                view.TimePart = dto.ToLocalTime().TimeOfDay;
            }
        }

        private static void DatePartPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimeCompositeView view && e.NewValue is DateTimeOffset dt)
            {
                if (view.TimePart is TimeSpan time)
                {
                    view.DateTime = dt.Date.Add(time).ToUniversalTime();
                }
                else
                {
                    view.DateTime = dt.Date.ToUniversalTime();
                }
            }
        }

        private static void TimePartPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimeCompositeView view && e.NewValue is TimeSpan time)
            {
                if (view.DatePart is DateTimeOffset dt)
                {
                    view.DateTime = dt.Date.Add(time).ToUniversalTime();
                }
                else
                {
                    view.DateTime = System.DateTime.Now.Add(time).ToUniversalTime();
                }
            }
        }
    }
}
