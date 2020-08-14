/*******************************************************************************
  * Copyright 2020 Esri
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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    public sealed partial class TitleBar
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
                OfflineMapFlyout.Hide();
                OnlineMapFlyout.Hide();
                UserPanelFlyout.Hide();
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender?.SystemOverlayRightInset == null || sender?.Height == null)
            {
                return;
            }

            AppTitleBar.Height = sender.Height;
            // Future enhancement - update for RTL
            SystemButtonColumn.Width = new GridLength(Math.Max(sender.SystemOverlayRightInset - 50, 0));

            // Note: sometimes there is a crash when minimizing, suspected to be due to negative grid length
            if (sender.SystemOverlayRightInset < 50)
            {
                throw new Exception();
            }
        }

        public MainViewModel MainViewModel
        {
            get => (MainViewModel)GetValue(MainViewModelProperty);
            set => SetValue(MainViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for MainViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register(nameof(MainViewModel), typeof(MainViewModel), typeof(TitleBar), new PropertyMetadata(null));
    }
}
