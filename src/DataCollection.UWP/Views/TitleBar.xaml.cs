using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    public sealed partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();

            // Update the draggable title bar region
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // customize colors
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = (Application.Current.Resources["ChromeBackgroundBrush"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            titleBar.ButtonForegroundColor = (Application.Current.Resources["ChromeForegroundBrush"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            titleBar.ButtonHoverBackgroundColor = (Application.Current.Resources["ChromeBackgroundHoverBrush"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            titleBar.ButtonHoverForegroundColor = (Application.Current.Resources["BlueBrush"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;

            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += Instance_BroadcastMessengerValueChanged;
        }

        private void Instance_BroadcastMessengerValueChanged(object sender, BroadcastMessengerEventArgs e)
        {
            if (e.Args.Key == Shared.Models.BroadcastMessageKey.ClosePopups)
            {
                this.OfflineMapFlyout.Hide();
                this.OnlineMapFlyout.Hide();
                this.UserPanelFlyout.Hide();
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender?.SystemOverlayRightInset == null || sender?.Height == null)
            {
                return;
            }

            AppTitleBar.Height = sender.Height;
            // TODO - update for RTL
            SystemButtonColumn.Width = new GridLength(Math.Max(sender.SystemOverlayRightInset - 50, 0));

            // Note: sometimes there is a crash when minimizing, suspected to be due to negative grid length
            if (sender.SystemOverlayRightInset < 50)
            {
                throw new Exception();
            }
        }

        public MainViewModel MainViewModel
        {
            get { return (MainViewModel)GetValue(MainViewModelProperty); }
            set { SetValue(MainViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(TitleBar), new PropertyMetadata(null));


    }
}
