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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Views
{
    /// <summary>
    /// Interaction logic for IdentifiedFeaturePopup.xaml
    /// </summary>
    public sealed partial class IdentifiedFeaturePopup : UserControl, INotifyPropertyChanged
    {
        public IdentifiedFeaturePopup()
        {
            InitializeComponent();

            // set the IdentifiedFeatureViewModel property when DataContext changes and IdentifiedFeatureViewModel is set
            DataContextChanged += (s, e) =>
            {
                if (DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName == nameof(IdentifiedFeatureViewModel))
                        {
                            IdentifiedFeatureViewModel = mainViewModel.IdentifiedFeatureViewModel;
                        }
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
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event handler for user selecting to add a new attachment by capturing new media
        /// </summary>
        private async void CaptureMediaButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _identifiedFeatureViewModel.AttachmentsViewModel.NewAttachmentFile = await MediaHelper.RecordMediaAsync();
        }

        /// <summary>
        /// Event handler for user selecting to add a new attachment by browsing for a file
        /// </summary>
        private async void BrowseFilesButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _identifiedFeatureViewModel.AttachmentsViewModel.NewAttachmentFile = await MediaHelper.GetFileFromUser();
        }
    }
}