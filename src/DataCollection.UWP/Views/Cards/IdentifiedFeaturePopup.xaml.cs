/*******************************************************************************
  * Copyright 2019 Esri
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
using System.ComponentModel;
using Windows.UI.Xaml;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Helpers;
using Windows.UI.Xaml.Controls;
using System.Linq;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Cards
{
    /// <summary>
    /// Interaction logic for IdentifiedFeaturePopup.xaml
    /// </summary>
    public sealed partial class IdentifiedFeaturePopup
    {
        public IdentifiedFeaturePopup()
        {
            InitializeComponent();
        }

        public MainViewModel MainViewModel
        {
            get => (MainViewModel)GetValue(MainViewModelProperty);
            set => SetValue(MainViewModelProperty, value);
        }

        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register(nameof(MainViewModel), typeof(MainViewModel), typeof(IdentifiedFeaturePopup), new PropertyMetadata(null));

        /// <summary>
        /// Event handler for user selecting to add a new attachment by capturing new media
        /// </summary>
        private async void CaptureMediaButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                MainViewModel.IdentifyResultViewModel.CurrentlySelectedFeature.AttachmentsViewModel.NewAttachmentFile = await MediaHelper.RecordMediaAsync();
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Shared.Properties.Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);
            }
        }

        /// <summary>
        /// Event handler for user selecting to add a new attachment by browsing for a file
        /// </summary>
        private async void BrowseFilesButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                MainViewModel.IdentifyResultViewModel.CurrentlySelectedFeature.AttachmentsViewModel.NewAttachmentFile = await MediaHelper.GetFileFromUser();
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Shared.Properties.Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);
            }
        }

        private void PopupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView lv && lv.Tag is Flyout flyout && lv.DataContext is DestinationRelationshipViewModel vm)
            {
                if (e.AddedItems.Any())
                {
                    lv.ScrollIntoView(e.AddedItems.First());
                }
            }
        }

        private void PopupFlyout_Opened(object sender, object e)
        {
            if (sender is Flyout flyout && flyout.Content is Grid grid && grid.Children.OfType<ListView>().FirstOrDefault() is ListView lv 
                && lv.DataContext is DestinationRelationshipViewModel vm && vm.PopupManager != null)
            {
                lv.ScrollIntoView(vm.PopupManager);
            }
        }
    }
}