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
            titleBar.ButtonBackgroundColor = (Application.Current.Resources["chrome-background"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            titleBar.ButtonForegroundColor = (Application.Current.Resources["chrome-foreground"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            titleBar.ButtonHoverBackgroundColor = (Application.Current.Resources["chrome-hover-background"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            titleBar.ButtonHoverForegroundColor = (Application.Current.Resources["chrome-hover-foreground"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Height = sender.Height;
            // TODO - update for RTL
            SystemButtonColumn.Width = new GridLength(sender.SystemOverlayRightInset - 50);
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
