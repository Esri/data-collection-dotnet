/*******************************************************************************
  * Copyright 2019 Esri
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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    /// <summary>
    /// Interaction logic for OriginRelatedRecordPopup.xaml
    /// </summary>
    public sealed partial class OriginRelatedRecordPopup : UserControl, INotifyPropertyChanged
    {
        public OriginRelatedRecordPopup()
        {
            InitializeComponent();

            // set the OriginRelationshipViewModel property when DataContext changes and OriginRelationshipViewModel is set
            DataContextChanged += (s, e) =>
            {
                if (DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName == nameof(IdentifiedFeatureViewModel))
                            IdentifiedFeatureViewModel = mainViewModel.IdentifiedFeatureViewModel;
                    };
                }
            };
        }

        private IdentifiedFeatureViewModel _identifiedFeatureViewModel;

        /// <summary>
        /// Gets or sets the IdentifiedFeatureViewModel
        /// </summary>
        public IdentifiedFeatureViewModel IdentifiedFeatureViewModel
        {
            get => _identifiedFeatureViewModel;
            set
            {
                _identifiedFeatureViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        private async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Shared.Properties.Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);

                // This exception should never happen; if it does, the app is in an unknown state and should terminate.
                Environment.FailFast("Error invoking property change event handler.", ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event handler for user selecting to add a new attachment by capturing new media
        /// </summary>
        private async void CaptureMediaButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                _identifiedFeatureViewModel.SelectedOriginRelationship.AttachmentsViewModel.NewAttachmentFile = await MediaHelper.RecordMediaAsync();
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
                _identifiedFeatureViewModel.SelectedOriginRelationship.AttachmentsViewModel.NewAttachmentFile = await MediaHelper.GetFileFromUser();
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
    }
}
